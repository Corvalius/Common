Param(
  [string]$Stable="yes",  
  [string]$Development="no"  
)

Push-Location $PSScriptRoot

. ".\ManageBuildNumbers.ps1"

# Move to the parent (SolutionDir).
Push-Location ..

$nuGetGalleryServer = "https://www.myget.org/F/corvalius/api/v2/package"
$nugetApiKey = "YOUR-API-KEY-HERE"
$nugetExecutable = Get-ChildItem "NuGet.exe" -Recurse | Select -First 1 
Set-Alias Execute-NuGet $nugetExecutable

Update-BuildVersionFromRevision


Echo "Updating NuGet..."
Execute-NuGet Update -self

$buildNumber = Get-BuildVersion
Echo "Building current version $buildNumber ..."

# Setting the Build Server API Key
Execute-NuGet SetApiKey $nugetApiKey -Source $nuGetGalleryServer

$nuspecFiles = Get-ChildItem .\*.nuspec -Recurse 

$avoidDirectory = "packages"
foreach( $spec in $nuspecFiles )  
{            
        $dirs = ($spec.Directory.FullName).Split('\')            
        $targetIndex = 0;
        for($i = $dirs.Length - 1; $i -ge 0; $i--) 
        {            
            if ($dirs[$i].ToLower().StartsWith($avoidDirectory.ToLower())) 
            {            
                $targetIndex = $i            
                break            
            }            
        }
        
        if ($targetIndex -ne 0 )
        {
            continue;
        }

        Push-Location $spec.Directory

            Echo "Cleaning old packages..."

            # Ensure the directory is clean of packages.
            Del *.nupkg

            # Build the package.		
			$projectFile = $spec.BaseName + ".csproj"		
			
			if ($Stable -eq "yes")
			{
				Execute-NuGet pack $projectFile -Symbols -Build -Version $buildNumber
			}
			elseif ($Development -eq "no")
			{
				Execute-NuGet pack $projectFile -Symbols -Build -Version "$buildNumber-prerelease"
			}			            
			else
			{
				Execute-NuGet pack $projectFile -Symbols -Build -Version "$buildNumber-prerelease"
			}

            # Push the package.
            $nupkgFiles = Get-ChildItem .\*.nupkg -Recurse | Select -First 1 
            Execute-NuGet push $nupkgFiles -source $nuGetGalleryServer
    
        Pop-Location
}

Pop-Location
Pop-Location
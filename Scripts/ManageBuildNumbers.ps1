function Update-BuildVersionFromRevision
{  
    [CmdletBinding()]  
    param()  
          
    $assemblyPattern = "[0-9]+(\.([0-9]+|\*)){1,3}"  
    $assemblyVersionPattern = 'AssemblyVersion\("([0-9]+(\.([0-9]+|\*)){1,3})"\)'  
      
    $foundFiles = get-childitem .\*GlobalAssemblyInfo.cs -recurse  
                         
              
    $rawVersionNumberGroup = get-content $foundFiles | select-string -pattern $assemblyVersionPattern | select -first 1 | % { $_.Matches }              
    $rawVersionNumber = $rawVersionNumberGroup.Groups[1].Value  
                    
    $versionParts = $rawVersionNumber.Split('.')     
    [int]$currentVersion = 0;  
    [bool]$result = [int]::TryParse($versionParts[2], [ref]$currentVersion)
    $updatedAssemblyVersion = "{0}.{1}.{2}" -f $versionParts[0], $versionParts[1], ($currentVersion+1)
      
    $assemblyVersion  
                  
    foreach( $file in $foundFiles )  
    {     
        (Get-Content $file) | ForEach-Object {
                % {$_ -replace $assemblyPattern, $updatedAssemblyVersion }                 
            } | Set-Content $file        
    }  
}

function Get-BuildVersion  
{  
    [CmdletBinding()]  
    param()  
          
    $assemblyPattern = "[0-9]+(\.([0-9]+|\*)){1,3}"  
    $assemblyVersionPattern = 'AssemblyVersion\("([0-9]+(\.([0-9]+|\*)){1,3})"\)'  
      
    $foundFiles = get-childitem .\*GlobalAssemblyInfo.cs -recurse  
                         
              
    $rawVersionNumberGroup = get-content $foundFiles | select-string -pattern $assemblyVersionPattern | select -first 1 | % { $_.Matches }              
    $rawVersionNumber = $rawVersionNumberGroup.Groups[1].Value  


    $versionParts = $rawVersionNumber.Split('.')       
    $updatedAssemblyVersion = "{0}.{1}.{2}" -f $versionParts[0], $versionParts[1], $versionParts[2] 
      
    return $updatedAssemblyVersion
} 
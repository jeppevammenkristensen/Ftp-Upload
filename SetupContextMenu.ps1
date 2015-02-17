New-PSDrive -Name HKCR -PSProvider Registry -Root HKEY_CLASSES_ROOT

function Set-ContextMenuEntry(){
    param ([string] $path)

    $resolvedPath = Resolve-Path $path

    $resolvedPath = "$resolvedPath `"%L`" "

	Push-Location
    
    New-Item -Path HKCR:Directory\shell  -Name CustomFtp -Value "Upload til ftp" -Force
    New-Item -Path HKCR:Directory\shell\CustomFtp -Name command -Value  $resolvedPath -Force

    Pop-Location
}

Set-ContextMenuEntry .\Upload\bin\Debug\Upload.exe


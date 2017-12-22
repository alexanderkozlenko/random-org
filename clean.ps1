foreach ($directory in (get-childitem -path "$PSScriptRoot/" -directory -include @("bin", "obj") -recurse)) {
    write-output "Removing `"$directory`"...";
    remove-item -path $directory -recurse -force -ea silentlycontinue;
}
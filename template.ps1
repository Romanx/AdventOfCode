param (
    [string]$date = $(Get-Date -Format "yyyy-MM-dd")
)

dotnet run -p .\src\Template\Template.csproj --ignore-failed-sources -- $date
gci src/years/*.csproj -R | Sort CreationTime -Descending | Select-Object -First 1 | Foreach-Object { dotnet sln add $_ }
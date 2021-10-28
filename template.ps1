param (
    [string]$date = $(Get-Date -Format "yyyy-MM-dd")
)

dotnet run --project .\src\Template\Template.csproj -- --date $date
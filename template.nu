def main [str: string = ""] {
  let $date = if ($str | str trim) == "" {
    (date now | date format '%Y-%m-%d')
  } else {
    ($str | into datetime | date format '%Y-%m-%d')
  }

  dotnet run --project .\src\Template\Template.csproj -- --date $date
}
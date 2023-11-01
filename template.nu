def main [str: string = ""] {
  let $date = if ($str | str trim) == "" {
    (date now | format date '%Y-%m-%d')
  } else {
    ($str | into datetime | format date '%Y-%m-%d')
  }

  dotnet run --project .\src\Template\Template.csproj -- --date $date
}
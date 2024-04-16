A set of handy shell commands, primarily used to generate the coverage report

Generates dotnet coverage report:
dotnet test --collect:\"XPlat Code Coverage\" && reportgenerator \"-reports:**/TestResults/**/coverage.cobertura.xml\" -reporttypes:lcov -targetdir:.coverage

Generates documentation:
docfx metadata
docfx build --serve -n <domain name here>

.coverage/lcov.info

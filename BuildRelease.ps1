$ver = $args[0]
dotnet build --configuration Release --output "build/Release/dll_$ver" --version-suffix "$ver" "NSL.Management.CentralService.sln"
dotnet pack --configuration Release --output "build/Release/package_$ver" --version-suffix "$ver" "NSL.Management.CentralService.sln"
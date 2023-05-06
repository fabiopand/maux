# Maux

MAUX is a .NET MAUI extension library which provides solutions for common problems found when developing in MAUI.


## Maux.Mvvm

`Maux.Mvvm` extends what is already provided by `CommunityToolkit.Mvvm` by providing solutions and patterns to solve common issues found when working with MAUI.

Well known problems are:
- .NET MAUI doesn't give a clear understanding of how the DI works in relation to Shell Navigation, and [many developers are complaining about it](https://github.com/dotnet/maui/issues/7354)
- In Microsoft examples, they usually call static methods (i.e. `Shell.Current.GoToAsync`), while that's fast to type, it is also a bad practice considering unit-testing
- Shell navigation API is really limited (`Relative routing to shell elements is currently not supported`)

### Dispose page and all dependencies when navigating away

.NET MAUI doesn't give a clear understanding of how the DI works in relation to Navigation, and [many developers are complaining about it](https://github.com/dotnet/maui/issues/7354).

This library provides a clean way to define [Shell](https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/shell/) routes with `Scoped` page and page models (view-model).

Every page will be disposed when popped from the `Shell` navigation stack, and we can say the same for all the `Scoped` dependencies requested.


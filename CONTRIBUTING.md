# Contributing to Kookie

Guidelines for contributing to the Roslyn repo.

---

# Preamble

Thank you for wanting to contribute to the Kookie Programming Language! Before you start, please take a quick look at the guidelines we set for contributors, to ensure a smooth collaboration!

1. [Issues](#issues)
2. [Pull Requests](#pull-requests)
3. [Coding Styles](#coding-styles)

## Issues

- Use a descriptive title that identifies the issue to be addressed or the requested feature. For example, when describing an issue where the compiler is not behaving as expected, write your bug title in terms of what the compiler should do rather than what it is doing – “Compiler should report ERR1234 when Xyz is used in Abcd.”
- Specify a detailed description of the issue or requested feature.
- Provide the following for bug reports
    - Describe the expected behavior and the actual behavior. If it is not self-evident such as in the case of a crash, provide an explanation for why the expected behavior is expected.
    - Provide example code that reproduces the issue.
    - Specify any relevant exception messages and stack traces.
- Subscribe to notifications for the created issue in case there are any follow up questions.

## Pull Requests

If you worked on a new feature or fixed an open issue, please create a **pull request** so we can merge your work into ours.
If it is not immediately apparent which feature you added or which issue you addressed, please include an adequate description in your pull request. If you still need to work on the pull request before it can be merged please open it as a _draft_.
Note that if you do successfully terminate an issue, include `Closes #issue-id` in your commit message so the issue is automatically referenced.
Also do note that we try to enforce consistent coding styles among the repository, which we explain in the section [coding styles](#coding-styles). We review every pull request, and we would appreciate it if you would accept the changes that we propose during our reviews.

- Submit language feature requests as issues in the [Kookie language](https://github.com/kookielang/Kookielang) repo. Once a feature is championed and validated, we will help begin a prototype on this repo inside a feature branch.
- Don't submit language features as PRs to this repo first, or they will likely be declined.
- Submit issues for other features. This facilitates discussion of a feature separately from its implementation, and increases the acceptance rates for pull requests.
- Don't submit large code formatting changes without discussing with the team first.

These two blogs posts on contributing code to open source projects are good too: [Open Source Contribution Etiquette](http://tirania.org/blog/archive/2010/Dec-31.html) by Miguel de Icaza and [Don’t “Push” Your Pull Requests](https://www.igvita.com/2011/12/19/dont-push-your-pull-requests/) by Ilya Grigorik.

## Coding Styles

In general we follow the standard [Microsoft C# naming conventions](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/general-naming-conventions). If you want insights to individual variable naming conventions, please refer to the existing code in the repository. `.editorconfig` files are planned.

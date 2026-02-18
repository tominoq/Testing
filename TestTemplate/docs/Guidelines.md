# Guidelines

### Content:
- [Introduction](Introduction.md)
- [Prerequisites](Prerequisites.md)
- Guidelines
- [Running Tests](Running-Tests.md)
---


## Best practice

1. **Branching:**
	- Create a new branch for each feature or task. Use descriptive names (e.g., `feature/login-page`, `bugfix/fix-typo`).
2. **Error and Warning Resolution:**
	- Fix all build errors and warnings before committing code.
3. **Code Formatting:**
	- Run `dotnet format` to ensure code style consistency before pushing changes.
4. **Pull Requests:**
	- Submit pull requests for review. Ensure all checks pass before merging.
5. **Testing:**
	- Run all relevant tests locally and ensure they pass before pushing.
6. **Configuration Files:**
	- Do not commit sensitive or local configuration files (e.g., `appsettings.local.json`).
7. **Commit Messages:**
	- Use clear, concise commit messages describing the change.
8. **Test architecture:**
    - Do not mix business logic (what you want to test) with UI details (how you do it).
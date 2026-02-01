# Contributing to EFT Viewer

Thank you for your interest in contributing to EFT Viewer!

## Getting Started

1. Fork the repository
2. Clone your fork
3. Set up the development environment (see README.md)

## Git Workflow

This project uses a **develop → main** branching strategy:

- **All PRs target `develop`**, not main
- Feature branches are created from `develop`
- Releases are merged from `develop` to `main`

See [docs/guides/git-workflow.md](docs/guides/git-workflow.md) for detailed instructions.

### Quick Start

```bash
# Start from develop
git checkout develop
git pull origin develop

# Create your feature branch
git checkout -b feature/my-feature

# Make changes, commit, push
git push -u origin feature/my-feature

# Create PR targeting develop
gh pr create --base develop
```

## Code Standards

- Follow existing code style and patterns
- Add tests for new functionality
- Update documentation as needed
- Ensure all tests pass before submitting PR

## Pull Request Process

1. Create PR targeting `develop` branch
2. Fill out PR template
3. Ensure CI passes
4. Request review
5. Squash merge when approved

## Reporting Issues

- Use GitHub Issues for bug reports and feature requests
- Include steps to reproduce for bugs
- Include sample files if relevant (ensure no sensitive data)

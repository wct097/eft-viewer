# Contributing to EFT Viewer

Thank you for your interest in contributing to EFT Viewer!

## Getting Started

1. Fork the repository
2. Clone your fork
3. Set up the development environment (see README.md)

## Git Workflow

This project follows **GitHub Flow**: a single long-lived `main` branch with short-lived topic branches.

- **All PRs target `main`**
- Feature/fix/chore branches are created from `main`
- PRs are **squash-merged** into `main`
- Topic branches are deleted after merge

See [docs/guides/git-workflow.md](docs/guides/git-workflow.md) for detailed instructions.

### Quick Start

```bash
# Start from an up-to-date main
git checkout main
git pull origin main

# Create your topic branch
git checkout -b feature/my-feature

# Make changes, commit, push
git push -u origin feature/my-feature

# Open the PR targeting main
gh pr create --base main
```

## Code Standards

- Follow existing code style and patterns
- Add tests for new functionality
- Update documentation as needed
- Ensure all tests pass before submitting PR

## Pull Request Process

1. Open PR targeting `main`
2. Fill out PR template
3. Ensure CI passes
4. Request review
5. Squash merge when approved, then delete the branch

## Reporting Issues

- Use GitHub Issues for bug reports and feature requests
- Include steps to reproduce for bugs
- Include sample files if relevant (ensure no sensitive data)

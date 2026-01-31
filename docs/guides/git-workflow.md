# Git Workflow

This project uses a **develop → main** branching strategy with feature branches.

## Branch Structure

```
main        Production-ready releases only
  │
  └── develop     Integration branch for all work
        │
        ├── feature/xxx    New features
        ├── fix/xxx        Bug fixes
        └── chore/xxx      Maintenance tasks
```

## Rules

### Main Branch
- **Never commit directly to main**
- **Never merge feature branches to main**
- Only receives merges from `develop` for releases
- Protected branch - requires PR approval

### Develop Branch
- Integration branch for all development work
- All PRs target `develop`
- Should always be in a working state
- Merges to `main` for releases

### Feature Branches
- Branch from `develop`: `git checkout -b feature/xyz develop`
- PR targets `develop`
- Use squash merge to keep history clean
- Delete after merge

## Workflow

### Starting New Work

```bash
# Ensure develop is up to date
git checkout develop
git pull origin develop

# Create feature branch
git checkout -b feature/my-feature
```

### Submitting Work

```bash
# Push feature branch
git push -u origin feature/my-feature

# Create PR targeting develop
gh pr create --base develop
```

### Releasing to Main

```bash
# Ensure develop is ready
git checkout develop
git pull origin develop

# Merge to main
git checkout main
git pull origin main
git merge develop
git push origin main

# Tag the release
git tag v1.0.0
git push origin v1.0.0
```

## Common Mistakes to Avoid

| Mistake | Consequence | Prevention |
|---------|-------------|------------|
| PR to main instead of develop | Breaks branch sync | Always check `--base develop` |
| Branching from main | Missing develop changes | Always `git checkout -b feature/x develop` |
| Direct commits to main/develop | Bypasses review | Branch protection rules |

## Recovering from Mistakes

### PR merged to wrong branch (main instead of develop)
```bash
# Sync develop with main
git checkout develop
git merge main
git push origin develop
```

### Feature branch based on main instead of develop
```bash
# Rebase onto develop
git checkout feature/my-feature
git rebase --onto develop main feature/my-feature
git push --force-with-lease
```

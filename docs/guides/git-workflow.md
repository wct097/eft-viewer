# Git Workflow

This project uses a **develop → main** branching strategy with feature branches.

## ⛔ CRITICAL RULES

```
╔═══════════════════════════════════════════════════════════════════════════════╗
║  MANDATORY - VIOLATING THESE RULES CORRUPTS THE REPOSITORY                    ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║  1. ALL pull requests target `develop` - NEVER target `main` directly         ║
║  2. ALL feature/fix branches are created FROM `develop`                       ║
║  3. ONLY `develop` merges into `main` (for releases)                          ║
║  4. NEVER delete the `develop` branch                                         ║
║  5. NEVER squash merge into `main` - use regular merge only                   ║
║  6. ALWAYS squash merge into `develop`                                        ║
╚═══════════════════════════════════════════════════════════════════════════════╝
```

## Branch Structure

```
main (protected)     ← Releases only, NEVER direct PRs
  │
  └── develop        ← ALL work merges here first
        │
        ├── feature/xxx    New features
        ├── fix/xxx        Bug fixes
        └── chore/xxx      Maintenance tasks
```

## Merge Strategy

| Source | Target | Merge Type | Why |
|--------|--------|------------|-----|
| feature/* | develop | **Squash** | Clean history, one commit per feature |
| fix/* | develop | **Squash** | Clean history, one commit per fix |
| develop | main | **Regular merge** | Preserve milestone history |

## GitHub Branch Protection Rules

Configure these rules in **Settings → Rules → Rulesets**:

### Rule: `main` branch
- ✅ Require pull request before merging
- ✅ Require approvals (1+)
- ✅ Block force pushes
- ✅ Block deletions
- ❌ Do NOT allow squash merging (regular merge only)

### Rule: `develop` branch
- ✅ Require pull request before merging
- ✅ Block force pushes
- ✅ Block deletions
- ✅ Require squash merging

## Workflow

### Starting New Work

```bash
# ALWAYS start from develop
git checkout develop
git pull origin develop

# Create feature branch FROM develop
git checkout -b feature/my-feature
```

### Submitting Work

```bash
# Push feature branch
git push -u origin feature/my-feature

# Create PR - ALWAYS target develop
gh pr create --base develop
```

### Releasing to Main

```bash
# Create PR from develop → main
gh pr create --base main --head develop --title "Release vX.Y.Z"

# IMPORTANT: Use regular merge, NOT squash merge
# After PR is merged:
git checkout main
git pull origin main

# Tag the release
git tag vX.Y.Z
git push origin vX.Y.Z

# Sync tag back to develop (optional but recommended)
git checkout develop
git pull origin develop
git merge main
git push origin develop
```

## Common Mistakes

| Mistake | Consequence | Prevention |
|---------|-------------|------------|
| PR to main instead of develop | Skips integration, breaks sync | Branch rules + always use `--base develop` |
| Squash merge to main | Loses develop history link | Branch rules: disable squash for main |
| Branching from main | Missing develop changes | Always `checkout develop` first |
| Deleting develop | Catastrophic - loses integration branch | Branch rules: block deletion |
| Direct commits to protected branches | Bypasses review | Branch rules: require PR |

## Recovering from Mistakes

### PR was squash-merged to main (should have been regular merge)

**This is bad** - the develop branch history is now disconnected from main.

```bash
# Reset main to before the bad merge
git checkout main
git reset --hard <commit-before-merge>
git push --force origin main  # Requires temporarily disabling protection

# Re-do the merge correctly
gh pr create --base main --head develop --title "Release vX.Y.Z"
# Use regular merge (not squash) when merging the PR
```

### PR merged to main instead of develop

```bash
# Reset main if needed, then sync develop
git checkout develop
git merge origin/main
git push origin develop
```

### Feature branch based on main instead of develop

```bash
git checkout feature/my-feature
git rebase --onto develop main feature/my-feature
git push --force-with-lease
```

### Develop branch was deleted

```bash
# If you have it locally:
git push origin develop

# If not, recreate from main and cherry-pick/reapply work
git checkout main
git checkout -b develop
git push origin develop
```

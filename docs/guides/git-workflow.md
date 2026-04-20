# Git Workflow

This project follows **GitHub Flow**: a single long-lived `main` branch with short-lived topic branches that are squash-merged back into `main` and deleted.

## ⛔ CRITICAL RULES

```
╔═══════════════════════════════════════════════════════════════════════════════╗
║  MANDATORY RULES                                                              ║
╠═══════════════════════════════════════════════════════════════════════════════╣
║  1. `main` is the only long-lived branch                                      ║
║  2. ALL work happens on short-lived `feature/*`, `fix/*`, `chore/*` branches  ║
║  3. ALL pull requests target `main`                                           ║
║  4. ALL merges into `main` are **squash merges**                              ║
║  5. Delete the topic branch (local + remote) after the PR merges              ║
║  6. No direct pushes to `main` — always go through a PR                       ║
╚═══════════════════════════════════════════════════════════════════════════════╝
```

## Branch Structure

```
main (protected, only long-lived branch)
  │
  ├── feature/xxx    New features
  ├── fix/xxx        Bug fixes
  └── chore/xxx      Maintenance tasks
```

Topic branches are short-lived: create from `main`, do the work, open a PR, squash-merge, delete.

## Merge Strategy

| Source | Target | Merge Type | Why |
|--------|--------|------------|-----|
| feature/* | main | **Squash** | One commit per feature on `main` |
| fix/* | main | **Squash** | One commit per fix on `main` |
| chore/* | main | **Squash** | One commit per chore on `main` |

## GitHub Branch Protection Rules

Configure in **Settings → Rules → Rulesets** for the `main` branch:

- ✅ Require pull request before merging
- ✅ Require approvals (1+)
- ✅ Require status checks to pass
- ✅ Block force pushes
- ✅ Block deletions
- ✅ Allow **squash merging only** (disable regular merge and rebase merge)
- ✅ Automatically delete head branches after merge

## Workflow

### Starting New Work

```bash
# Always start from an up-to-date main
git checkout main
git pull origin main

# Create a topic branch from main
git checkout -b feature/my-feature
```

### Submitting Work

```bash
# Push the topic branch
git push -u origin feature/my-feature

# Open the PR — always target main
gh pr create --base main
```

### After the PR Merges

```bash
git checkout main
git pull origin main

# Clean up the merged topic branch
git branch -d feature/my-feature
# If GitHub didn't auto-delete the remote:
git push origin --delete feature/my-feature
```

### Releasing

```bash
# Tag the release directly from main
git checkout main && git pull origin main
git tag vX.Y.Z
git push origin vX.Y.Z
```

Use a release PR (e.g. `chore/release-vX.Y.Z`) if the release requires version bumps, changelog updates, or other file changes before tagging.

## Common Mistakes

| Mistake | Consequence | Prevention |
|---------|-------------|------------|
| Committing directly to `main` | Bypasses review and CI | Branch rules: require PR |
| Branching from a stale `main` | Avoidable merge conflicts | Always `git pull origin main` before branching |
| Non-squash merge into `main` | Clutters history with WIP commits | Branch rules: allow squash only |
| Leaving merged branches around | Branch list rot | Delete after merge; enable auto-delete |
| Long-lived topic branches | Drift, conflicts, stale reviews | Keep PRs small; rebase onto `main` often |

## Recovering from Mistakes

### Accidentally committed to `main` locally

```bash
# Move the commits to a new branch, reset main
git branch feature/rescue-work
git reset --hard origin/main
git checkout feature/rescue-work
# Now open a PR as normal
```

### Topic branch fell behind `main`

```bash
git checkout feature/my-feature
git fetch origin
git rebase origin/main
git push --force-with-lease
```

### PR merged with the wrong strategy (e.g. regular merge instead of squash)

Typically no action needed once merged — the change is on `main`. Tighten branch protection to prevent recurrence.

# Security Scan Command

## Overview
The `/security-scan` command runs comprehensive security validation to detect malicious code patterns, exposed secrets, and dependency vulnerabilities.

## Usage
```
/security-scan [--detailed]
```

## Options
- `--detailed`: Show detailed findings in the output

## What It Scans

### Malicious Code Patterns
- Command injection attempts
- Network exfiltration
- Reverse shells
- File system manipulation
- Credential theft
- Encoding/obfuscation

### Exposed Secrets
- API keys (AWS, Google, GitHub, etc.)
- Passwords in code
- Authentication tokens
- Private keys

### Dependency Vulnerabilities
- npm packages (via npm audit)
- Python packages (via safety/pip-audit)
- Go modules (via nancy)

## Security Levels

### Critical
- Exposed secrets
- Active malicious code
- Critical dependency vulnerabilities

### High
- Suspicious code patterns
- Potential security risks
- High-severity vulnerabilities

### Medium
- Code quality issues with security implications
- Medium-severity vulnerabilities

### Low
- Best practice violations
- Low-severity vulnerabilities

## Integration

### Manual Scanning
Run the security scanner directly:
```bash
./scripts/validation/security_scanner.sh
```

### Claude Code Integration
Use the `/security-scan` command during development to check for security issues before committing.

### CI/CD Integration
Add to your GitHub Actions or other CI/CD pipeline:
```yaml
- name: Security Scan
  run: ./scripts/validation/security_scanner.sh
```

## Response to Findings

### Critical Issues
1. **STOP** - Do not deploy or commit
2. **REMOVE** - Delete or fix the security issue
3. **VERIFY** - Re-run scan to confirm resolution

### High Risk Issues
1. **REVIEW** - Examine the context
2. **ASSESS** - Determine if it's a false positive
3. **FIX** - Address legitimate issues

### Medium/Low Issues
1. **PLAN** - Schedule remediation
2. **TRACK** - Document in issue tracker
3. **IMPROVE** - Update code over time

## False Positives

If the scanner flags legitimate code:
1. Review the specific pattern
2. Confirm it's safe in your context
3. Consider refactoring to avoid the pattern
4. Document why it's safe if keeping

## Best Practices

1. **Run regularly** - Before each commit
2. **Fix immediately** - Don't accumulate security debt
3. **Stay updated** - Keep security tools current
4. **Train team** - Share security awareness

---

*Security scanning is essential for maintaining code integrity and protecting sensitive data.*

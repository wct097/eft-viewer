#!/bin/bash

# security_scanner.sh - Security vulnerability scanning validation
# Part of AI Setup Enhanced Validation Framework
# Version: 1.0.0
# Performs comprehensive security scanning for malicious code detection

set -euo pipefail

# Script directory and dependencies
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VALIDATION_ROOT="$SCRIPT_DIR"

# Module-specific configuration
CONFIG_FILE="$VALIDATION_ROOT/validation_config.json"
SECURITY_LOG_FILE="$VALIDATION_ROOT/../logs/security_scan_$(date +%Y%m%d_%H%M%S).log"
RESULTS_FILE="$VALIDATION_ROOT/../logs/security_results.json"

# Create logs directory if it doesn't exist
mkdir -p "$(dirname "$SECURITY_LOG_FILE")"

# Logging function
log_security() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] SECURITY: $*" | tee -a "$SECURITY_LOG_FILE"
}

# Security patterns for detection
MALICIOUS_PATTERNS=(
    # Network exfiltration patterns
    "curl.*-d.*\$"
    "wget.*--post-data"
    "nc.*-e"
    "socat.*exec"
    
    # Command injection patterns
    "eval.*\$("
    "system\([\"'].*\$"
    "subprocess.*shell=True"
    "os\.system\("
    "\$\(.*\)"
    "\`.*\`"
    
    # File system manipulation
    "rm.*-rf.*/"
    "chmod.*777"
    "chown.*root"
    "dd.*if=/dev/zero"
    
    # Credential theft patterns
    "\/etc\/passwd"
    "\/etc\/shadow"
    "\.ssh\/id_rsa"
    "\.aws\/credentials"
    "\.env.*password"
    
    # Reverse shell patterns
    "bash.*-i.*>&"
    "python.*socket.*connect"
    "perl.*socket"
    "ruby.*socket"
    "php.*fsockopen"
    
    # Encoding/obfuscation patterns
    "base64.*-d"
    "echo.*\|.*base64"
    "printf.*\\\\x"
    "uuencode"
    "xxd.*-r"
    
    # Cryptocurrency mining
    "xmrig"
    "cryptonight"
    "stratum\+tcp"
    "monero"
    "mining.*pool"
)

# Secrets patterns
SECRETS_PATTERNS=(
    # API Keys
    "AKIA[0-9A-Z]{16}"  # AWS Access Key
    "AIza[a-zA-Z0-9_-]{35}"  # Google API Key
    "ghp_[a-zA-Z0-9]{36}"  # GitHub Personal Access Token
    "sk_live_[a-zA-Z0-9]{24,}"  # Stripe Live Key
    
    # Generic secrets
    "[Pp]assword['\"]?\s*[:=]\s*['\"][^'\"]{8,}"
    "[Aa]pi[_-]?[Kk]ey['\"]?\s*[:=]\s*['\"][^'\"]{10,}"
    "[Ss]ecret['\"]?\s*[:=]\s*['\"][^'\"]{10,}"
    "[Tt]oken['\"]?\s*[:=]\s*['\"][^'\"]{10,}"
)

# Initialize security scanner
init_security_scanner() {
    log_security "Initializing security scanner..."
    
    # Create results structure
    echo '{
        "scan_timestamp": "'$(date -Iseconds)'",
        "scan_type": "security",
        "findings": [],
        "summary": {
            "total_files_scanned": 0,
            "malicious_patterns_found": 0,
            "secrets_found": 0,
            "high_risk_files": 0
        }
    }' > "$RESULTS_FILE"
    
    log_security "Security scanner initialized"
}

# Scan for malicious patterns
scan_malicious_patterns() {
    log_security "Scanning for malicious code patterns..."
    
    local findings=0
    local scanned_files=0
    
    # Find all relevant files to scan
    while IFS= read -r file; do
        ((scanned_files++))
        local file_findings=0
        
        # Check each malicious pattern
        for pattern in "${MALICIOUS_PATTERNS[@]}"; do
            if grep -n -E "$pattern" "$file" 2>/dev/null; then
                log_security "WARNING: Malicious pattern found in $file: $pattern"
                ((findings++))
                ((file_findings++))
                
                # Add to results
                jq --arg file "$file" \
                   --arg pattern "$pattern" \
                   --arg severity "high" \
                   '.findings += [{"type": "malicious_pattern", "file": $file, "pattern": $pattern, "severity": $severity}]' \
                   "$RESULTS_FILE" > "$RESULTS_FILE.tmp" && mv "$RESULTS_FILE.tmp" "$RESULTS_FILE"
            fi
        done
        
        if [[ $file_findings -gt 0 ]]; then
            log_security "File $file contains $file_findings malicious patterns"
        fi
    done < <(find . -type f \( -name "*.sh" -o -name "*.py" -o -name "*.js" -o -name "*.php" -o -name "*.rb" \) -not -path "./.git/*" -not -path "./node_modules/*" 2>/dev/null)
    
    # Update summary
    jq --arg scanned "$scanned_files" \
       --arg findings "$findings" \
       '.summary.total_files_scanned = ($scanned | tonumber) | .summary.malicious_patterns_found = ($findings | tonumber)' \
       "$RESULTS_FILE" > "$RESULTS_FILE.tmp" && mv "$RESULTS_FILE.tmp" "$RESULTS_FILE"
    
    log_security "Malicious pattern scan complete: $findings patterns found in $scanned_files files"
    return $findings
}

# Scan for secrets
scan_secrets() {
    log_security "Scanning for exposed secrets..."
    
    local secrets_found=0
    
    # Check each secrets pattern
    for pattern in "${SECRETS_PATTERNS[@]}"; do
        while IFS= read -r match; do
            if [[ -n "$match" ]]; then
                local file=$(echo "$match" | cut -d: -f1)
                log_security "WARNING: Potential secret found in $file"
                ((secrets_found++))
                
                # Add to results
                jq --arg file "$file" \
                   --arg pattern "$pattern" \
                   --arg severity "critical" \
                   '.findings += [{"type": "exposed_secret", "file": $file, "pattern": $pattern, "severity": $severity}]' \
                   "$RESULTS_FILE" > "$RESULTS_FILE.tmp" && mv "$RESULTS_FILE.tmp" "$RESULTS_FILE"
            fi
        done < <(grep -r -E "$pattern" . --include="*.sh" --include="*.py" --include="*.js" --include="*.env" --include="*.yml" --include="*.yaml" --include="*.json" --exclude-dir=.git --exclude-dir=node_modules 2>/dev/null || true)
    done
    
    # Update summary
    jq --arg secrets "$secrets_found" \
       '.summary.secrets_found = ($secrets | tonumber)' \
       "$RESULTS_FILE" > "$RESULTS_FILE.tmp" && mv "$RESULTS_FILE.tmp" "$RESULTS_FILE"
    
    log_security "Secrets scan complete: $secrets_found potential secrets found"
    return $secrets_found
}

# Scan dependencies for vulnerabilities
scan_dependencies() {
    log_security "Scanning dependencies for vulnerabilities..."
    
    local vulns_found=0
    
    # NPM/Node.js
    if [[ -f "package.json" ]]; then
        log_security "Scanning npm dependencies..."
        if command -v npm &> /dev/null; then
            if npm audit --json 2>/dev/null | jq -e '.vulnerabilities | to_entries | length > 0' &>/dev/null; then
                log_security "WARNING: Vulnerable npm dependencies found"
                ((vulns_found++))
            fi
        fi
    fi
    
    # Python
    if [[ -f "requirements.txt" ]]; then
        log_security "Scanning Python dependencies..."
        if command -v safety &> /dev/null; then
            if ! safety check --json 2>/dev/null; then
                log_security "WARNING: Vulnerable Python dependencies found"
                ((vulns_found++))
            fi
        elif command -v pip-audit &> /dev/null; then
            if ! pip-audit 2>/dev/null; then
                log_security "WARNING: Vulnerable Python dependencies found"
                ((vulns_found++))
            fi
        fi
    fi
    
    # Go
    if [[ -f "go.mod" ]]; then
        log_security "Scanning Go dependencies..."
        if command -v go &> /dev/null; then
            if go list -json -m all | nancy sleuth 2>/dev/null | grep -q "Vulnerable"; then
                log_security "WARNING: Vulnerable Go dependencies found"
                ((vulns_found++))
            fi
        fi
    fi
    
    return $vulns_found
}

# Generate security report
generate_security_report() {
    local exit_code=$1
    
    log_security "Generating security report..."
    
    # Get summary stats
    local total_findings=$(jq '.findings | length' "$RESULTS_FILE")
    local high_severity=$(jq '[.findings[] | select(.severity == "high")] | length' "$RESULTS_FILE")
    local critical_severity=$(jq '[.findings[] | select(.severity == "critical")] | length' "$RESULTS_FILE")
    
    # Determine overall risk level
    local risk_level="low"
    if [[ $critical_severity -gt 0 ]]; then
        risk_level="critical"
    elif [[ $high_severity -gt 0 ]]; then
        risk_level="high"
    elif [[ $total_findings -gt 0 ]]; then
        risk_level="medium"
    fi
    
    # Update results with risk assessment
    jq --arg risk "$risk_level" \
       --arg status $([ $exit_code -eq 0 ] && echo "passed" || echo "failed") \
       '.summary.risk_level = $risk | .summary.scan_status = $status' \
       "$RESULTS_FILE" > "$RESULTS_FILE.tmp" && mv "$RESULTS_FILE.tmp" "$RESULTS_FILE"
    
    # Display summary
    echo ""
    echo "🛡️  SECURITY SCAN SUMMARY"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━"
    echo "Risk Level: $risk_level"
    echo "Total Findings: $total_findings"
    echo "Critical Issues: $critical_severity"
    echo "High Risk Issues: $high_severity"
    echo ""
    echo "Detailed results: $RESULTS_FILE"
    echo "Full log: $SECURITY_LOG_FILE"
    
    log_security "Security report generated"
}

# Main security validation
main() {
    local exit_code=0
    
    log_security "Starting security validation scan..."
    
    # Initialize scanner
    init_security_scanner
    
    # Run scans
    if ! scan_malicious_patterns; then
        exit_code=1
    fi
    
    if ! scan_secrets; then
        exit_code=1
    fi
    
    if ! scan_dependencies; then
        exit_code=1
    fi
    
    # Generate report
    generate_security_report $exit_code
    
    if [[ $exit_code -eq 0 ]]; then
        log_security "✅ Security validation completed successfully"
    else
        log_security "❌ Security validation found issues"
    fi
    
    return $exit_code
}

# Run main
main "$@"

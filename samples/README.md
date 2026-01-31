# Sample EFT Files

This directory is a placeholder for sample Electronic Fingerprint Transmission (EFT) files.

## Obtaining Test Files

### Option 1: NIST Reference Files (Recommended)

The NIST Standard References page includes data samples that may contain ready-to-use ANSI/NIST format files:

1. Visit [ANSI/NIST-ITL Standard References](https://www.nist.gov/itl/iad/image-group/ansinist-itl-standard-references)
2. Download `ansi-nist_2011_workshop_files.zip` ("IEPDs, schemas, data samples")
3. Also try `ansi-nist_character_separated_reference_files.zip` ("30 reference examples")
4. Extract and look for `.eft`, `.an2`, or `.nist` files

### Option 2: NIST Special Database 300

SD300 contains fingerprint images from deceased subjects (public domain):

1. Visit https://nigos.nist.gov/datasets/sd300/request
2. Register and request access
3. Download SD300a (6.4GB, PNG format, 500 ppi)

**Note:** SD300 contains PNG images, not packaged EFT files. You would need to use NBIS tools to convert and package them into ANSI/NIST format.

### Option 3: Create EFT from PNG using NBIS

If you have PNG fingerprint images, use [NIST Biometric Image Software (NBIS)](https://www.nist.gov/services-resources/software/nist-biometric-image-software-nbis):

1. Download NBIS source code from the NIST Biometric Open Source Server
2. Build the tools (requires C compiler)
3. Use `cwsq` to convert PNG to WSQ format
4. Use `an2k` utilities to package into ANSI/NIST format

## Licensing

NIST data is public domain under 15 U.S.C. § 105. Redistribution is permitted with attribution.

**Required citation for SD300:**
> Fiumara G, Flanagan P, Grantham J, Bandini B, Ko K, Libert J. NIST Special Database 300: Uncompressed Plain and Rolled Images from Fingerprint Cards. NIST Technical Note 1993. https://doi.org/10.6028/NIST.TN.1993

## File Format Reference

EFT/ANSI-NIST files use these common extensions:
- `.eft` - Electronic Fingerprint Transmission
- `.an2` - ANSI/NIST Type-2 format
- `.nist` - Generic NIST biometric format

See the project README for more information on the ANSI/NIST-ITL format structure.

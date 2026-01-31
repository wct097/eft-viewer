# Sample EFT Files

This directory contains sample Electronic Fingerprint Transmission (EFT) files for testing and demonstration.

## Included Samples

### nist-type-4-14-flats.eft

A sample containing both legacy Type-4 and modern Type-14 fingerprint records:

- **Type-1**: Transaction information (ANSI/NIST-ITL 1-2000 format)
- **Type-2**: Descriptive text
- **Type-4**: Legacy grayscale fingerprint images (WSQ compressed)
- **Type-14**: Variable-resolution fingerprint image

Size: 262 KB

### nist-type-14-tpcard.eft

A ten-print card sample with Type-14 fingerprint records:

- **Type-1**: Transaction information
- **Type-2**: Descriptive text
- **Type-14**: Multiple fingerprint images with quality metrics (NQM)

Size: 660 KB

## Attribution

These sample files are from the [NIST ANSI/NIST-ITL 2011 Workshop Files](https://www.nist.gov/itl/iad/image-group/ansinist-itl-standard-references), provided by the National Institute of Standards and Technology (NIST).

As works of the U.S. federal government, these files are in the **public domain** and not subject to copyright (17 U.S.C. § 105).

## Additional Test Data

For more comprehensive testing, NIST provides additional resources:

- [ANSI/NIST-ITL Standard References](https://www.nist.gov/itl/iad/image-group/ansinist-itl-standard-references) - More sample files and schemas
- [NIST Special Database 300](https://www.nist.gov/itl/iad/image-group/nist-special-database-300) - Fingerprint images from cards
- [NIST Special Database 302](https://www.nist.gov/itl/iad/image-group/nist-special-database-302) - Nail-to-nail fingerprint images

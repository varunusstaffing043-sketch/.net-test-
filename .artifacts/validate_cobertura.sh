#!/usr/bin/env bash
set -euo pipefail

FILE="tests/TodoApi.Tests/TestResults/coverage.cobertura.xml"
if [ -f "$FILE" ]; then
  echo "Validating Cobertura XML: $FILE"
  if command -v xmllint >/dev/null 2>&1; then
    if xmllint --noout "$FILE" 2>/dev/null; then
      echo "Cobertura XML is well-formed"
    else
      echo "Cobertura XML is invalid - removing to avoid Sonar parsing failure"
      rm -f "$FILE" || true
    fi
  else
    if command -v python >/dev/null 2>&1; then
      python - <<PY
import sys
import xml.etree.ElementTree as ET
import os
path = r"%s"
try:
    ET.parse(path)
    print('Cobertura XML is well-formed')
except Exception as e:
    print('Cobertura XML is invalid:', e)
    try:
        os.remove(path)
        print('Removed invalid Cobertura file to avoid Sonar parsing failure')
    except Exception as re:
        print('Failed to remove invalid cobertura file:', re)
        sys.exit(1)
PY
    else
      echo "No xmllint or python available; skipping Cobertura validation"
    fi
  fi
else
  echo "No Cobertura file to validate"
fi

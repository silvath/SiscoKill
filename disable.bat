reg DELETE "HKLM\Software\Microsoft\StrongName\Verification" /f
reg ADD "HKLM\Software\Microsoft\StrongName\Verification\*,*" /f
reg DELETE "HKLM\Software\Wow6432Node\Microsoft\StrongName\Verification" /f
reg ADD "HKLM\Software\Wow6432Node\Microsoft\StrongName\Verification\*,*" /f
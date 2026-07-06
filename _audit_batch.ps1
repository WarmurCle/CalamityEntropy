# Temporary audit script: count SpriteBatch open/close calls per file.
# Opens: .Begin( | BeginDrawingWithMode( | begin_( | BeginDefault( | BeginShader( | BeginDrawVertex(
# Closes: .End()
# Neutral helpers (End+Begin inside): EnterShaderRegion, ExitShaderRegion, UseBlendState, UseSampleState,
#   UseAdditive, UseState_UI, ReSetToBeginShader, ReSetToEndShader, DrawGlow, drawTextureToPoint
$root = "c:\Users\Hommeng\Documents\My Games\Terraria\tModLoader\ModSources\CalamityEntropy"
$files = Get-ChildItem -Path $root -Recurse -Filter *.cs | Where-Object {
    $_.FullName -notmatch '\\(bin|obj|\.vs|\.git)\\'
}
$results = @()
foreach ($f in $files) {
    $text = Get-Content -Raw -LiteralPath $f.FullName
    if ($null -eq $text) { continue }
    # strip line comments and block comments to reduce false positives
    $text = [regex]::Replace($text, '(?s)/\*.*?\*/', '')
    $text = [regex]::Replace($text, '(?m)//.*$', '')
    $opens = ([regex]::Matches($text, '\.Begin\s*\(')).Count
    $opens += ([regex]::Matches($text, 'BeginDrawingWithMode\s*\(')).Count
    $opens += ([regex]::Matches($text, '\.begin_\s*\(')).Count
    $opens += ([regex]::Matches($text, '\.BeginDefault\s*\(')).Count
    $opens += ([regex]::Matches($text, '\.BeginShader\s*\(')).Count
    $opens += ([regex]::Matches($text, '\.BeginDrawVertex\s*\(')).Count
    $closes = ([regex]::Matches($text, '\.End\s*\(\s*\)')).Count
    # subtract declarations in CEUtils itself (definitions contain the calls legitimately)
    if ($opens -ne $closes) {
        $rel = $f.FullName.Substring($root.Length + 1)
        $results += [pscustomobject]@{ File = $rel; Opens = $opens; Closes = $closes; Delta = $opens - $closes }
    }
}
$results | Sort-Object Delta | Format-Table -AutoSize | Out-String -Width 200

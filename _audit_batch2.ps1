# For each file with any End/Begin, print the ordered sequence of batch ops with line numbers,
# so pairing can be verified. Neutral helpers counted as (end+begin) pairs are listed too.
$root = "c:\Users\Hommeng\Documents\My Games\Terraria\tModLoader\ModSources\CalamityEntropy"
$patterns = @(
    @{ Name = 'END   '; Rx = '\.End\s*\(\s*\)' },
    @{ Name = 'BEGIN '; Rx = '\.Begin\s*\(' },
    @{ Name = 'BEGIN(mode) '; Rx = 'BeginDrawingWithMode\s*\(' },
    @{ Name = 'BEGIN(begin_) '; Rx = '\.begin_\s*\(' },
    @{ Name = 'BEGIN(default) '; Rx = '\.BeginDefault\s*\(' },
    @{ Name = 'BEGIN(shader) '; Rx = '\.BeginShader\s*\(' },
    @{ Name = 'BEGIN(vertex) '; Rx = '\.BeginDrawVertex\s*\(' },
    @{ Name = 'SWAP(enter) '; Rx = '\.EnterShaderRegion\s*\(' },
    @{ Name = 'SWAP(exit) '; Rx = '\.ExitShaderRegion\s*\(' },
    @{ Name = 'SWAP(blend) '; Rx = '\.UseBlendState\s*\(' },
    @{ Name = 'SWAP(sample) '; Rx = '\.UseSampleState\s*\(' },
    @{ Name = 'SWAP(add) '; Rx = '\.UseAdditive\s*\(' },
    @{ Name = 'SWAP(ui) '; Rx = '\.UseState_UI\s*\(|\.UseBlendState_UI\s*\(|\.UseSampleState_UI\s*\(' },
    @{ Name = 'SWAP(reset) '; Rx = 'ReSetToBeginShader\s*\(|ReSetToEndShader\s*\(' },
    @{ Name = 'GLOW '; Rx = 'CEUtils\.DrawGlow\s*\(|[^.\w]DrawGlow\s*\(' },
    @{ Name = 'TEXPOINT '; Rx = 'drawTextureToPoint\s*\(' },
    @{ Name = 'PIXPREP '; Rx = 'EffectLoader\.PreparePixelShader\s*\(' },
    @{ Name = 'PIXAPPLY '; Rx = 'EffectLoader\.ApplyPixelShader\s*\(' }
)
$files = Get-ChildItem -Path $root -Recurse -Filter *.cs | Where-Object {
    $_.FullName -notmatch '\\(bin|obj|\.vs|\.git)\\' -and $_.Name -ne 'CEUtils.cs'
}
foreach ($f in $files) {
    $lines = Get-Content -LiteralPath $f.FullName
    $hits = @()
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        # skip comment-only lines
        if ($line -match '^\s*//') { continue }
        foreach ($p in $patterns) {
            if ($line -match $p.Rx) {
                $hits += ('{0,5}: {1} {2}' -f ($i + 1), $p.Name, $line.Trim())
                break
            }
        }
    }
    if ($hits.Count -gt 0) {
        $rel = $f.FullName.Substring($root.Length + 1)
        Write-Output "=== $rel"
        $hits | ForEach-Object { Write-Output $_ }
    }
}

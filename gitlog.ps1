git log -n 50 --pretty=format:'%h|%H|%an|%ae|%aI|%cn|%ce|%cI|%D|%s' |
  ForEach-Object {
    $f = $_ -split '\|',10
    [PSCustomObject]@{
      ShortHash       = $f[0]
      FullHash        = $f[1]
      AuthorName      = $f[2]
      AuthorEmail     = $f[3]
      AuthorDate      = [datetime]::Parse($f[4])
      CommitterName   = $f[5]
      CommitterEmail  = $f[6]
      CommitDate      = [datetime]::Parse($f[7])
      Refs            = $f[8]   # e.g. (HEAD -> main, tag: v1.2.3)
      Subject         = $f[9]
    }
  } |
  Format-Table -AutoSize
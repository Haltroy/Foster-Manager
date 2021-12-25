# Shortcuts

To avoid having a long command, you can use shortcuts to certain types of URLs.
Here, you can find all the implemented URL shortcuts.

## Autofill

Foster & HTU-Man can auto-fill these fields for more automation:

- `[VERNAME]` : The version name, available since the first shortcuts.
- `[VERNO]` : The version number, available since the first shortcuts.
- `[PROJNAME]` : The project name, available since the first shortcuts.
- `[ARCH]` : The architecture name, available since the first shortcuts.
- `((more coming soon))`

## Shortcut List

### The First Shortcuts (HTAlt b1.7.2 & HTU-Man 1.0.0.1)

Fields marked with `*`are required by schemes.

#### GitHub

`github:[user name*]/[repository name*]/[branch]/[File]`

Grabs `[project name].Foster` from a GitHub repository as raw content if `[File]` is not specified.

#### GitHub Releases

`githubrelease:[user name*]/[repository name*]/[tag]/[file]`

Grabs `[project name].Foster` from latest release of a GitHub repository as raw content if `[Tag]` and `[File]`.

#### GitLab

`gitlab:[user name*]/[repository name*]/[branch]/[file]`

Grabs `[project name].Foster` from a GitLab repository as raw content.

#### GitLab Releases

`gitlabrelease:[user name*]/[repository name*]/[tag]/[file]`

Grabs `[project name].Foster` from latest release of a GitLab repository as raw content.

#### Google Drive

`gdrive:[File ID*]`
`gdrv:[File ID*]`

Grabs the file located in the hot link ID from a Google Drive.


#### OneDrive

`1drive:[File ID*]`
`onedrive:[File ID*]`
`1drv:[File ID*]`

Grabs the file located in the hot link ID from a OneDrive.

#### HTTP/HTTPS

`http:[domain name*]/[file path]`
`https:[domain name*]/[file path]`

Grabs `[domain name].com/[project name].Foster` as raw content.

#### FTP

`ftp:[username]:[password]@[domain name*]:[port]/[file path]`

Grabs `[domain name].com/[project name].Foster` as raw content.

#### Embedded

`base:[input]`
`base64:[input]`

Converts `[input]` to bytes.

### The Second Shortcuts (HTALt b1.8+ & HTU-Man 1.0.0.2+)

`((planned))`

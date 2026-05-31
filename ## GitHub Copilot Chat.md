## GitHub Copilot Chat

- Extension: 0.49.0 (prod)
- VS Code: 1.121.0 (f6cfa2ea2403534de03f069bdf160d06451ed282)
- OS: win32 10.0.26200 x64
- GitHub Account: mas301

## Network

User Settings:
```json
  "http.systemCertificatesNode": true,
  "github.copilot.advanced.debug.useElectronFetcher": true,
  "github.copilot.advanced.debug.useNodeFetcher": false,
  "github.copilot.advanced.debug.useNodeFetchFetcher": true
```

Connecting to https://api.github.com:
- DNS ipv4 Lookup: timed out after 10 seconds
- DNS ipv6 Lookup: Error (13 ms): getaddrinfo ENOTFOUND api.github.com
- Proxy URL: None (0 ms)
- Electron fetch (configured): timed out after 10 seconds
- Node.js https: timed out after 10 seconds
- Node.js fetch: HTTP 200 (111 ms)

Connecting to https://api.githubcopilot.com/_ping:
- DNS ipv4 Lookup: 140.82.112.21 (15 ms)
- DNS ipv6 Lookup: Error (17 ms): getaddrinfo ENOTFOUND api.githubcopilot.com
- Proxy URL: None (1 ms)
- Electron fetch (configured): HTTP 200 (368 ms)
- Node.js https: HTTP 200 (362 ms)
- Node.js fetch: HTTP 200 (340 ms)

Connecting to https://copilot-proxy.githubusercontent.com/_ping:
- DNS ipv4 Lookup: 4.249.131.160 (9 ms)
- DNS ipv6 Lookup: Error (11 ms): getaddrinfo ENOTFOUND copilot-proxy.githubusercontent.com
- Proxy URL: None (1291 ms)
- Electron fetch (configured): HTTP 200 (505 ms)
- Node.js https: HTTP 200 (491 ms)
- Node.js fetch: HTTP 200 (484 ms)

Connecting to https://mobile.events.data.microsoft.com: HTTP 404 (151 ms)
Connecting to https://dc.services.visualstudio.com: HTTP 404 (659 ms)
Connecting to https://copilot-telemetry.githubusercontent.com/_ping: HTTP 200 (367 ms)
Connecting to https://telemetry.individual.githubcopilot.com/_ping: HTTP 200 (372 ms)
Connecting to https://default.exp-tas.com: HTTP 400 (406 ms)

Number of system certificates: 99

## Documentation

In corporate networks: [Troubleshooting firewall settings for GitHub Copilot](https://docs.github.com/en/copilot/troubleshooting-github-copilot/troubleshooting-firewall-settings-for-github-copilot).
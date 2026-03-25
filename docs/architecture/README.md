# Architecture Diagram Starter

This folder gives you a ready starter for architecture and code hierarchy diagrams.

## Structure
- `mermaid/` : markdown-friendly diagrams (works in many viewers)
- `plantuml/` : text-based UML diagrams for robust architecture docs

## Recommended VS Code Plugins
1. Draw.io: `hediet.vscode-drawio`
2. PlantUML: `jebbs.plantuml`
3. Mermaid Preview: `bierner.markdown-mermaid`

## Quick Start
1. Edit Mermaid files in `mermaid/*.mmd`
2. Edit PlantUML files in `plantuml/*.puml`
3. Keep diagrams versioned with code changes

## Suggested Update Rule
Whenever backend modules or deployment flow changes:
1. Update system context diagram
2. Update deployment diagram
3. Update code layer hierarchy diagram
4. Update sequence diagram for critical flows (login, leave approval)

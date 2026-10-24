# Texture paths

All retheme PNGs belong under `Textures/Anneli_Retheme_DarkSouls/`. The texture-path changes are in the 1.7 patch file for each Def type. Keep directional suffixes (`_north`, `_east`, `_south`), mask suffix `m`, and stack-count filenames unchanged.

| Texture path under `Anneli_Retheme_DarkSouls/` | Artwork present |
| --- | --- |
| `Anneli_Building/Fountain` | North/east/south images and masks |
| `Anneli_Building/PuppetTheatre` | North/east/south images and masks |
| `Anneli_Effect/PupaFormationA`, `B`, `C` | Three pupation-start flecks |
| `Anneli_Effect/PupaFinishA` | Pupation-finish fleck |
| `Anneli_Effect/Anneli_Infest_Particle` | Infest particle |
| `Anneli_Item/Anthelmintic` | Single and bulk images |
| `Anneli_Item/Egg` | Outsider egg random variants; starts with placeholder A |
| `Anneli_Item/Egg_Colonist` | Colonist egg random variants; starts with placeholder A |
| `Anneli_Item/NutrientSolution` | Single and bulk images |
| `Anneli_Item/Pupa` | Pupa random variants; starts with placeholder A |
| `Anneli_Larva/Larva` | Default north/east/south images |
| `Anneli_Larva/Larva1` through `Larva5` | Five alternate north/east/south sets |
| `Anneli_Larva/Dessicated_Larva` | Dessicated larva east image |
| `Anneli_UI/Anneli_Infect` | Ability icon |

The larva patch removes base variants 6–8 from the PawnKindDef and sets the body draw size to 1.25. The dessicated graphic now points to this retheme's `Dessicated_Larva_east.png`, matching the base mod's east-only naming pattern.

For egg and pupa folders, add only finished variants. If you have five egg images, place A–E in the corresponding folder; `Graphic_Random` will use those five. Replace the placeholder A rather than leaving unused base-art copies.

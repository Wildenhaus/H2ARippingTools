# H2ARippingTools

Experimental tool for ripping Halo 2 Anniversary (Campaign) assets.

Help/Research:
  - Zatarita
  - sleepzay
  - Unordinal

# Features
 * ✔️ List .pck file contents 
 * ✔️ Extract raw files from .pck
 * ➖ Extract DDS textures (currently broken)
 * ➖ Extract models (some may not work work)
   * ✔️ Meshes
   * ✔️ UVs
   * ✖️ Vertex Normals
   * ✖️ Bones/Skinning
   * ✖️ Materials
 
Your mileage may vary

# Usage
Filters are delimited by '|', no wildcards. Ex: plasma|.tpl

  H2ARipper.exe list PATH_TO_PCK --filter FILTERS_GO_HERE
  H2ARipper.exe extract PATH_TO_PCK OUTPUT_PATH --filter FILTERS_GO_HERE
  H2ARipper.exe convert PATH_TO_PCK --outpath OUTPUT_PATH --filter FILTERS_GO_HERE

#

![](https://i.imgur.com/vfmyI4n.png)

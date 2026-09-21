"""Draws the window textures of the city buildings (Assets/Art/city_windows_base.png and city_windows_emission.png).

Run it from the project folder:  python Tools/GenerateCityTextures.py

The picture holds four tiles side by side. One tile is one unit of building width and one unit of building height,
with 8 columns and 12 rows of windows. The top of every tile has a glowing neon line, and the left edge of every
tile has a dark strip of wall. BuildingMesh.cs points the roofs and the antenna tips at these two spots, so the
numbers below must stay in step with the constants in that script.
"""
import os
import random

import numpy as np
from PIL import Image

TILES = 4
TILE_WIDTH = 512
TILE_HEIGHT = 576
COLUMNS = 8
ROWS = 12
CELL_WIDTH = TILE_WIDTH // COLUMNS
CELL_HEIGHT = TILE_HEIGHT // ROWS
MARGIN = 6
NEON_HEIGHT = 16

WARM = (255, 210, 110)
AMBER = (255, 165, 75)
SOFT_WHITE = (255, 238, 205)
COOL_WHITE = (200, 230, 255)
CYAN = (110, 235, 255)
MAGENTA = (255, 120, 230)

# For each tile: the colour of the wall, the colour of the neon line, and how often each light colour is used.
STYLES = [
    {"wall": (34, 40, 60), "neon": CYAN, "lights": [(WARM, 5), (AMBER, 2), (SOFT_WHITE, 2), (COOL_WHITE, 1)]},
    {"wall": (30, 38, 54), "neon": CYAN, "lights": [(COOL_WHITE, 45), (SOFT_WHITE, 30), (WARM, 20), (CYAN, 5)]},
    {"wall": (26, 34, 56), "neon": MAGENTA, "lights": [(CYAN, 45), (COOL_WHITE, 20), (MAGENTA, 15), (WARM, 20)]},
    {"wall": (38, 36, 56), "neon": CYAN, "lights": [(AMBER, 45), (WARM, 33), (MAGENTA, 5), (SOFT_WHITE, 17)]},
]


def pick_light(rng, lights):
    colours = [colour for colour, _ in lights]
    weights = [weight for _, weight in lights]
    return rng.choices(colours, weights)[0]


def draw_tile(index, style):
    rng = random.Random(100 + index)
    base = np.zeros((TILE_HEIGHT, TILE_WIDTH, 3), np.float32)
    emission = np.zeros((TILE_HEIGHT, TILE_WIDTH, 3), np.float32)

    base[:, :] = style["wall"]
    base += np.random.default_rng(index).normal(0, 1.5, base.shape).astype(np.float32)

    # A lighter line between every three floors.
    for floor_line in range(0, TILE_HEIGHT, CELL_HEIGHT * 3):
        base[floor_line:floor_line + 3, :] = np.array(style["wall"]) * 1.35

    for row in range(ROWS):
        lit_chance = rng.uniform(0.4, 0.85)

        for column in range(COLUMNS):
            x0 = column * CELL_WIDTH + MARGIN
            y0 = row * CELL_HEIGHT + MARGIN
            x1 = (column + 1) * CELL_WIDTH - MARGIN
            y1 = (row + 1) * CELL_HEIGHT - MARGIN
            base[y0:y1, x0:x1] = (12, 16, 28)

            if rng.random() > lit_chance:
                continue

            colour = np.array(pick_light(rng, style["lights"]), np.float32)
            brightness = rng.uniform(0.7, 1.0)
            gradient = np.linspace(1.0, 0.75, y1 - y0)[:, None, None]
            emission[y0:y1, x0:x1] = colour * brightness * gradient
            base[y0:y1, x0:x1] = colour * 0.3

    # The neon line along the top edge of the tile.
    fade = np.linspace(0.6, 1.0, NEON_HEIGHT // 2)
    fade = np.concatenate([fade, fade[::-1]])[:, None, None]
    emission[:NEON_HEIGHT, :] = np.array(style["neon"], np.float32) * fade
    base[:NEON_HEIGHT, :] = np.array(style["neon"], np.float32) * 0.25

    return np.clip(base, 0, 255).astype(np.uint8), np.clip(emission, 0, 255).astype(np.uint8)


def main():
    base_tiles = []
    emission_tiles = []

    for index, style in enumerate(STYLES):
        base, emission = draw_tile(index, style)
        base_tiles.append(base)
        emission_tiles.append(emission)

    folder = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "Assets", "Art")
    Image.fromarray(np.concatenate(base_tiles, axis=1)).save(os.path.join(folder, "city_windows_base.png"))
    Image.fromarray(np.concatenate(emission_tiles, axis=1)).save(os.path.join(folder, "city_windows_emission.png"))
    print("Wrote the window textures:", TILES, "tiles of", TILE_WIDTH, "x", TILE_HEIGHT)


if __name__ == "__main__":
    main()

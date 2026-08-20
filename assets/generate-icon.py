from pathlib import Path
from PIL import Image, ImageDraw

sizes = (16, 20, 24, 32, 40, 48, 64, 128, 256)
images = []
for size in sizes:
    scale = size / 256
    image = Image.new("RGBA", (size, size))
    draw = ImageDraw.Draw(image)
    box = tuple(round(value * scale) for value in (12, 12, 244, 244))
    draw.rounded_rectangle(box, radius=round(52 * scale), fill="#0A0A0A")
    bars = ((58, 72, 184), (88, 48, 208), (118, 84, 172), (148, 60, 196), (178, 38, 218), (208, 78, 178))
    width = max(1, round(13 * scale))
    for x, top, bottom in bars:
        draw.line((round(x * scale), round(top * scale), round(x * scale), round(bottom * scale)), fill="white", width=width)
    images.append(image)

output = Path(__file__).with_name("RukaCut.ico")
images[-1].save(output, format="ICO", sizes=[(size, size) for size in sizes])
images[-1].save(Path(__file__).with_name("RukaCut.png"))

uniform sampler2D texture;
uniform float ratio;
uniform float x;
uniform float y;
uniform float r;

void main() {
	vec2 coords = gl_TexCoord[0].xy;
	vec4 pixel = texture2D(texture, coords);

	if (distance(vec2(coords.x * ratio, coords.y), vec2(x * ratio, y)) < r) {
		gl_FragColor = vec4(1.0 - pixel.r, 1.0 - pixel.g, 1.0 - pixel.b, pixel.a);
	} else {
		gl_FragColor = texture2D(texture, coords);
	}
}
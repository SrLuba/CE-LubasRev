#version 110

uniform sampler2D texture;
uniform vec2 textureSize;

vec4 texelFetch(vec2 uv) {
	return texture2D(
		texture, vec2(
			gl_TexCoord[0].x + (uv.x / textureSize.x),
			gl_TexCoord[0].y + (uv.y / textureSize.y)
		)
	);
}

void main() {
	vec4 pixel = texture2D(texture, gl_FragCoord.xy);

	if (pixel.a <= 0.01) {
		vec2 size = textureSize;

		float sum = 0.0;
		// Check for non-diagonal edges.
		sum += texelFetch(vec2(-1.0, 0)).a;
		sum += texelFetch(vec2(0, -1.0)).a;
		sum += texelFetch(vec2(1.0, 0)).a;
		sum += texelFetch(vec2(0, 1.0)).a;
		// Check transparency.
		if (sum > 0.0) {
			gl_FragColor = vec4(0.75, 0.0, 0.0, 0.75);
			return;
		}
		// Check for diagonal edges.
		sum += texelFetch(vec2(-1.0, -1.0)).a;
		sum += texelFetch(vec2(-1.0, 1.0)).a;
		sum += texelFetch(vec2(1.0, -1.0)).a;
		sum += texelFetch(vec2(1.0, 1.0)).a;
		if (sum > 0.0) {
			gl_FragColor = vec4(0.75, 0.0, 0.0, 0.5);
			return;
		}
		gl_FragColor = vec4(0.0, 0.0, 0.0, 0.0);
		return;
	}
	gl_FragColor = vec4(0.0, 0.0, 0.0, 0.0);
}
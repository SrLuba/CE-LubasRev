uniform sampler2D texture;

void main() {
	vec4 pixel = texture2D(texture, gl_TexCoord[0].xy);

	if (pixel.a == 0.0) {
		gl_FragColor = pixel;
		return;
	}
	float colorFactor = 0.5;
	float colorFactorR = 1.0 - colorFactor;
	float cLinear = pixel.r * 0.2126 + pixel.g * 0.7152 + pixel.b * 0.0722;

	vec4 targetColor = vec4(
		pixel.r * colorFactorR + cLinear * colorFactor,
		pixel.g * colorFactorR + cLinear * colorFactor,
		pixel.b * colorFactorR + cLinear * colorFactor,
		pixel.a
	);
	gl_FragColor = vec4(
		targetColor.r + 0.25,
		targetColor.g + 0.25,
		targetColor.b * 0.9,
		pixel.a
	);
}
uniform sampler2D texture;

void main()
{
   vec4 pixel = texture2D(texture, gl_TexCoord[0].xy);
   
   if(pixel.a == 0.0f)
   {
      gl_FragColor = pixel;
   }
   else
   {
      gl_FragColor = vec4(245.0f, 245.0f, 245.0f, pixel.a);
   }
}
Shader "Outline/Transparent" {
	Properties{
		_color("Color", Color) = (1,1,1,0.5)
	}

		SubShader{
		Tags{ "Queue" = "Geometry+1" }
		Pass{
		Blend SrcAlpha OneMinusSrcAlpha
		Lighting On
		ZWrite On

		Material{
		Diffuse[_color]
	}
	}
	}
}
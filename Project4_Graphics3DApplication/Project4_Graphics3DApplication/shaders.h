#pragma once

const char* vertexShaderSource = R"(
	#version 330 core
	layout (location = 0) in vec3 aPos;
    layout (location = 1) in vec3 aColor;
	layout (location = 2) in vec3 aNormal;

	out vec3 ourColor;
	out vec3 Normal;
	out vec3 FragPos;

    uniform mat4 model;
	uniform mat4 view;
	uniform mat4 projection;

	void main()
	{
		FragPos = vec3(model * vec4(aPos, 1.0));
		Normal = mat3(transpose(inverse(model))) * aNormal;
		gl_Position = projection * view * vec4(FragPos, 1.0);
		ourColor = aColor;
	}
)";

const char* fragmentShaderSource = R"(
	#version 330 core
	out vec4 FragColor;

	in vec3 ourColor;
	in vec3 Normal;
	in vec3 FragPos;

	uniform int renderMode;
	uniform vec3 lightPos;
	uniform vec3 viewPos;

	uniform vec3 lanternPos;
	uniform vec3 lanternColor;

	uniform vec3 objectColor;

	void main()
	{
		if (renderMode == 0)
		{
			FragColor = vec4(ourColor, 1.0f);
		}
		else if (renderMode == 1) // Sun + Lantern lighting
		{
			vec3 norm = normalize(Normal);					// N
			vec3 viewDir = normalize(viewPos - FragPos);	// V
		    vec3 objColor = ourColor;                       // IO

			float ambientStrength = 0.1;
			vec3 ambient = ambientStrength * vec3(1.0f, 1.0f, 1.0f); // Ambient component

		    // Sunlight lighting
			vec3 lightDir = normalize(lightPos - FragPos);	// L
			vec3 sunColor = vec3(1.0f, 1.0f, 0.5f);      // Sunlight	
			
			float diffuseStrength = 0.8;
			float diff = max(dot(norm, lightDir), 0.0);
			vec3 sunDiffuse = diffuseStrength * diff * sunColor; // Diffuse component from Sun

			float specularStrength = 0.2;
			vec3 reflectDir = reflect(-lightDir, norm);		// R

			float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
			vec3 sunSpecular = specularStrength * spec * sunColor; // Specular component from Sun

			// Lantern lighting 
			vec3 lanternDir = normalize(lanternPos - FragPos);	// L
            float distance = length(lanternPos - FragPos); // Distance to lantern
			float attenuation = 1.0 / (1.0 + 0.22 * distance + 0.0019 * distance * distance); 

			float diff2 = max(dot(norm, lanternDir), 0.0);
			vec3 lanternDiffuse = diffuseStrength * diff2 * lanternColor; // Diffuse component from Lantern

			vec3 reflectDir2 = reflect(-lanternDir, norm);		// R
			float spec2 = pow(max(dot(viewDir, reflectDir2), 0.0), 32);
			vec3 lanternSpecular = specularStrength * spec2 * lanternColor; // Specular component from Lantern

		    vec3 lanternResult = (lanternDiffuse + lanternSpecular) * attenuation;
			vec3 sunResult = sunDiffuse + sunSpecular;


			vec3 result = (lanternResult + sunResult + ambient) * objColor;

			FragColor = vec4(result, 1.0f);
		}
		else if (renderMode == 2)
        {
             FragColor = vec4(objectColor, 1.0f); // For light source
        }
	}
)";
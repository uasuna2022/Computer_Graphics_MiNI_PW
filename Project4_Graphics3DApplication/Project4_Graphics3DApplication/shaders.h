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

	void main()
	{
		if (renderMode == 0)
		{
			FragColor = vec4(ourColor, 1.0f);
		}
		else if (renderMode == 1)
		{
			vec3 lightColor = vec3(1.0, 1.0, 1.0);			// IL
			vec3 norm = normalize(Normal);					// N
			vec3 lightDir = normalize(lightPos - FragPos);	// L
		    vec3 objColor = vec3(1.0, 1.0, 1.0);            // IO
			
			float diff = max(dot(norm, lightDir), 0.0);
			vec3 diffuse = diff * lightColor;

			float specularStrength = 0.5;
			vec3 viewDir = normalize(viewPos - FragPos);	// V
			vec3 reflectDir = reflect(-lightDir, norm);		// R

			float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
			vec3 specular = specularStrength * spec * lightColor;

			vec3 result = (diffuse + specular) * objColor;

			FragColor = vec4(result, 1.0f);
		}
	}
)";
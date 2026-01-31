#include <glad/glad.h>
#include <GLFW/glfw3.h>

#include <iostream>
#include <vector>
#include <cmath>
#include "config.h"
#include "shaders.h"

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>

const float PI = 3.14159265358979323846F;
int cameraMode = 0; // 0 - basic fixed camera observing a scene, 1 - fixed camera following the moving sphere,
                    // 2 - camera connected with the moving sphere (third-person perspective)

void key_callback(GLFWwindow* window, int key, int scancode, int action, int mods)
{
	if (action == GLFW_PRESS)
	{
		if (key == GLFW_KEY_RIGHT)
		{
			cameraMode = (cameraMode + 1) % 3;
			std::cout << "Camera mode switched to: " << cameraMode << std::endl;
		}
		if (key == GLFW_KEY_LEFT)
		{
			cameraMode = (cameraMode + 2) % 3;
			std::cout << "Camera mode switched to: " << cameraMode << std::endl;
		}
		if (key == GLFW_KEY_ESCAPE)
		{
			glfwSetWindowShouldClose(window, true);
			std::cout << "Escape pressed. Exiting program.\n";
		}
	}
}

void generateTorus(std::vector<float>& vertices, std::vector<unsigned int>& indices,
	float mainRadius, float tubeRadius,
	int mainSegments, int tubeSegments) {

	// Firstly we generate vertices and colors
	for (int i = 0; i <= mainSegments; i++) {
		float theta = (float)i / mainSegments * 2.0f * PI; 
		float cosTheta = cos(theta);
		float sinTheta = sin(theta);

		for (int j = 0; j <= tubeSegments; j++) {
			float phi = (float)j / tubeSegments * 2.0f * PI;
			float cosPhi = cos(phi);
			float sinPhi = sin(phi);

			float x = (mainRadius + tubeRadius * cosPhi) * cosTheta;
			float y = (mainRadius + tubeRadius * cosPhi) * sinTheta;
			float z = tubeRadius * sinPhi;

			float cx = mainRadius * cosTheta;
			float cy = mainRadius * sinTheta;
			float cz = 0;

			glm::vec3 normal = glm::normalize(glm::vec3(x - cx, y - cy, z - cz));

			// Pushing position (x,y,z)
			vertices.push_back(x);
			vertices.push_back(y);
			vertices.push_back(z);

			// PUshing color (mapped to [0,1] range)
			vertices.push_back(0.5f + 0.5f * cosTheta); 
			vertices.push_back(0.5f + 0.5f * cosPhi);   
			vertices.push_back(0.5f + 0.5f * sinTheta); 

			// Pushing normal (nx, ny, nz)
			vertices.push_back(normal.x);
			vertices.push_back(normal.y);
			vertices.push_back(normal.z);
		}
	}

	// Then we generate indices
	for (int i = 0; i < mainSegments; i++) {
		for (int j = 0; j < tubeSegments; j++) {
			int currentRow = i * (tubeSegments + 1);
			int nextRow = (i + 1) * (tubeSegments + 1);

			int p1 = currentRow + j;
			int p2 = nextRow + j;
			int p3 = nextRow + j + 1;
			int p4 = currentRow + j + 1;

			// Generating two triangles for each quad on the torus surface
			indices.push_back(p1);
			indices.push_back(p2);
			indices.push_back(p4);

			indices.push_back(p2);
			indices.push_back(p3);
			indices.push_back(p4);
		}
	}
}

void generateSphere(float radius, int sectorCount, int stackCount, std::vector<float>& vertices, std::vector<unsigned int>& indices) {
	float x, y, z, xy;                              
	float nx, ny, nz, lengthInv = 1.0f / radius;    
	float sectorStep = 2 * PI / sectorCount;
	float stackStep = PI / stackCount;
	float sectorAngle, stackAngle;

	for (int i = 0; i <= stackCount; i++) {
		stackAngle = PI / 2 - i * stackStep;        // starting from pi/2 to -pi/2
		xy = radius * cosf(stackAngle);             // r * cos(u)
		z = radius * sinf(stackAngle);              // r * sin(u)

		for (int j = 0; j <= sectorCount; j++) {
			sectorAngle = j * sectorStep;           // starting from 0 to 2pi

			// vertex position (x, y, z)
			x = xy * cosf(sectorAngle);             // r * cos(u) * cos(v)
			y = xy * sinf(sectorAngle);             // r * cos(u) * sin(v)

			vertices.push_back(x);
			vertices.push_back(y);
			vertices.push_back(z);

			vertices.push_back(1.0f);
			vertices.push_back(1.0f);
			vertices.push_back(1.0f);

			nx = x * lengthInv;
			ny = y * lengthInv;
			nz = z * lengthInv;
			vertices.push_back(nx);
			vertices.push_back(ny);
			vertices.push_back(nz);
		}
	}

	// generate indices
	int k1, k2;
	for (int i = 0; i < stackCount; ++i) {
		k1 = i * (sectorCount + 1);     // beginning of current stack
		k2 = k1 + sectorCount + 1;      // beginning of next stack

		for (int j = 0; j < sectorCount; ++j, ++k1, ++k2) {
			if (i != 0) {
				indices.push_back(k1);
				indices.push_back(k2);
				indices.push_back(k1 + 1);
			}
			if (i != (stackCount - 1)) {
				indices.push_back(k1 + 1);
				indices.push_back(k2);
				indices.push_back(k2 + 1);
			}
		}
	}
}

int main()
{
	glfwInit();
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 3);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 3);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);

	GLFWwindow* window = glfwCreateWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "3D Graphics Application", NULL, NULL);
	if (window == NULL)
	{
		std::cout << "Failed to create GLFW window" << std::endl;
		glfwTerminate();
		return -1;
	}

	glfwMakeContextCurrent(window);
	glfwSetKeyCallback(window, key_callback);

	if (!gladLoadGLLoader((GLADloadproc)glfwGetProcAddress))
	{
		std::cout << "Failed to initialize GLAD" << std::endl;
		return -1;
	}

	glEnable(GL_DEPTH_TEST);

	
	unsigned int vertexShader = glCreateShader(GL_VERTEX_SHADER);
	glShaderSource(vertexShader, 1, &vertexShaderSource, NULL);
	glCompileShader(vertexShader);

    unsigned int fragmentShader = glCreateShader(GL_FRAGMENT_SHADER);
	glShaderSource(fragmentShader, 1, &fragmentShaderSource, NULL);
	glCompileShader(fragmentShader);

	unsigned int shaderProgram = glCreateProgram();
	glAttachShader(shaderProgram, vertexShader);
	glAttachShader(shaderProgram, fragmentShader);
	glLinkProgram(shaderProgram);

	glDeleteShader(vertexShader);
	glDeleteShader(fragmentShader);

	// Floor generation
	float floorVertices[] = {
		 50.0f, -2.0f,  50.0f,  0.1f, 0.9f, 0.1f,         0.0f, 1.0f, 0.0f,
		-50.0f, -2.0f,  50.0f,  0.1f, 0.9f, 0.1f,         0.0f, 1.0f, 0.0f,
		-50.0f, -2.0f, -50.0f,  0.1f, 0.9f, 0.1f,         0.0f, 1.0f, 0.0f,
		 50.0f, -2.0f, -50.0f,  0.1f, 0.9f, 0.1f,         0.0f, 1.0f, 0.0f
	};

	unsigned int floorIndices[] = {
		0, 1, 2,
		0, 2, 3  
	};

	unsigned int VBO_Floor, VAO_Floor, EBO_Floor;
	glGenVertexArrays(1, &VAO_Floor);
	glGenBuffers(1, &VBO_Floor);
	glGenBuffers(1, &EBO_Floor);

	glBindVertexArray(VAO_Floor);

	glBindBuffer(GL_ARRAY_BUFFER, VBO_Floor);
	glBufferData(GL_ARRAY_BUFFER, sizeof(floorVertices), floorVertices, GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO_Floor);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(floorIndices), floorIndices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	// Tetraid generation
	float tetraVertices[] = {
		 0.0f,  1.0f,  0.0f,   1.0f, 0.0f, 0.0f,   0.0f, 0.5f, 0.8f,
		 -1.0f, -1.0f,  1.0f,   1.0f, 0.0f, 0.0f,   0.0f, 0.5f, 0.8f,
		 1.0f, -1.0f,  1.0f,   1.0f, 0.0f, 0.0f,   0.0f, 0.5f, 0.8f,
		 0.0f,  1.0f,  0.0f,   0.0f, 1.0f, 0.0f,   0.8f, 0.5f, -0.4f,
		 1.0f, -1.0f,  1.0f,   0.0f, 1.0f, 0.0f,   0.8f, 0.5f, -0.4f,
		 0.0f, -1.0f, -1.0f,   0.0f, 1.0f, 0.0f,   0.8f, 0.5f, -0.4f,
		 0.0f,  1.0f,  0.0f,   0.0f, 0.0f, 1.0f,  -0.8f, 0.5f, -0.4f,
		 0.0f, -1.0f, -1.0f,   0.0f, 0.0f, 1.0f,  -0.8f, 0.5f, -0.4f,
		 -1.0f, -1.0f,  1.0f,   0.0f, 0.0f, 1.0f,  -0.8f, 0.5f, -0.4f,
		 -1.0f, -1.0f,  1.0f,   0.5f, 0.5f, 0.5f,   0.0f, -1.0f, 0.0f,
		 0.0f, -1.0f, -1.0f,   0.5f, 0.5f, 0.5f,   0.0f, -1.0f, 0.0f,
		 1.0f, -1.0f,  1.0f,   0.5f, 0.5f, 0.5f,   0.0f, -1.0f, 0.0f
	};

	unsigned int tetraVAO, tetraVBO;
	glGenVertexArrays(1, &tetraVAO);
	glGenBuffers(1, &tetraVBO);

	glBindVertexArray(tetraVAO);

	glBindBuffer(GL_ARRAY_BUFFER, tetraVBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(tetraVertices), tetraVertices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	// Torus generation
	std::vector<float> torusVerticesNew;
	std::vector<unsigned int> torusIndicesNew;

	generateTorus(torusVerticesNew, torusIndicesNew, 1.0f, 0.4f, 60, 30);

	unsigned int torusVAO, torusVBO, torusEBO;
	glGenVertexArrays(1, &torusVAO);
	glGenBuffers(1, &torusVBO);
	glGenBuffers(1, &torusEBO);

	glBindVertexArray(torusVAO);
	glBindBuffer(GL_ARRAY_BUFFER, torusVBO);
	glBufferData(GL_ARRAY_BUFFER, torusVerticesNew.size() * sizeof(float), torusVerticesNew.data(), GL_STATIC_DRAW);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, torusEBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, torusIndicesNew.size() * sizeof(unsigned int), torusIndicesNew.data(), GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	// Sphere generation
	std::vector<float> sphereVertices;
	std::vector<unsigned int> sphereIndices;

	generateSphere(1.0f, 30, 30, sphereVertices, sphereIndices);

	unsigned int sphereVAO, sphereVBO, sphereEBO;
	glGenVertexArrays(1, &sphereVAO);
	glGenBuffers(1, &sphereVBO);
	glGenBuffers(1, &sphereEBO);

	glBindVertexArray(sphereVAO);
	glBindBuffer(GL_ARRAY_BUFFER, sphereVBO);
	glBufferData(GL_ARRAY_BUFFER, sphereVertices.size() * sizeof(float), sphereVertices.data(), GL_STATIC_DRAW);
	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, sphereEBO);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sphereIndices.size() * sizeof(unsigned int), sphereIndices.data(), GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	//
	float cubeVertices[] = {
		-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
		 0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
		 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
		 0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
		-0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
		-0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

		-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,
		 0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 0.0f,
		 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
		 0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   1.0f, 1.0f,
		-0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 1.0f,
		-0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,   0.0f, 0.0f,

		-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
		-0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
		-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
		-0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
		-0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
		-0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

		 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
		 0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
		 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
		 0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
		 0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
		 0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

		-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
		 0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
		 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
		 0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
		-0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
		-0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

		-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
		 0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
		 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
		 0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
		-0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
		-0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
	};
	unsigned int cubeVAO, cubeVBO;
	glGenVertexArrays(1, &cubeVAO);
	glGenBuffers(1, &cubeVBO);

	glBindVertexArray(cubeVAO);
	glBindBuffer(GL_ARRAY_BUFFER, cubeVBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(cubeVertices), cubeVertices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);

	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);

	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	while (!glfwWindowShouldClose(window))
	{
		// Time and Sun position calculation
		float time = (float)glfwGetTime();
		float sunRadius = 50.0F;
		float daySpeed = 0.3F;

		float lightX = cos(time * daySpeed) * sunRadius;
		float lightZ = -10.0F;
		float lightY = sin(time * daySpeed) * sunRadius;

		// Sphere position calculation
		float sphereOrbitRadius = 5.0f;
		float sphereSpeed = 1.0f;
		float height = 3.5f;

		float sphereX = sin(time * sphereSpeed) * sphereOrbitRadius;
		float sphereY = height;
		float sphereZ = cos(time * sphereSpeed) * sphereOrbitRadius;
		glm::vec3 spherePos = glm::vec3(sphereX, sphereY, sphereZ);

		// Sky color calculation based on sun height
		float sunHeight = sin(time * daySpeed);
		float dayFactor = glm::clamp(sunHeight, 0.0F, 1.0F);

		glm::vec3 dayColor = glm::vec3(0.53F, 0.81F, 0.92F);
		glm::vec3 nigthColor = glm::vec3(0.1F, 0.1F, 0.2F);

		glm::vec3 skyColor = glm::mix(nigthColor, dayColor, dayFactor);

		glClearColor(skyColor.r, skyColor.g, skyColor.b, 1.0F);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		// Shader and uniforms setup
		glUseProgram(shaderProgram);
		int renderModeLoc = glGetUniformLocation(shaderProgram, "renderMode");
		int lightPosLoc = glGetUniformLocation(shaderProgram, "lightPos");
		int viewPosLoc = glGetUniformLocation(shaderProgram, "viewPos");

		int modelLoc = glGetUniformLocation(shaderProgram, "model");
		int viewLoc = glGetUniformLocation(shaderProgram, "view");
		int projLoc = glGetUniformLocation(shaderProgram, "projection");
		int objectColorLoc = glGetUniformLocation(shaderProgram, "objectColor");


		// Switching camera modes logic
		glm::vec3 cameraPos;
		glm::vec3 cameraTarget;
		glm::vec3 cameraUp = glm::vec3(0.0f, 1.0f, 0.0f);

		switch (cameraMode)
		{
			case 0:
				cameraPos = glm::vec3(0.0f, 10.0f, 20.0f);
				cameraTarget = glm::vec3(0.0f, 0.0f, 0.0f);
				break;
			case 1:
				cameraPos = glm::vec3(10.0f, 5.0f, 10.0f);
				cameraTarget = spherePos;
				break;
			case 2:
				cameraPos = spherePos + glm::vec3(0.0f, 10.0f, 0.0f);
				cameraTarget = glm::vec3(0.0f, 0.0f, 0.0f);
				break;
		}

		// View and projection matrices setup
		glm::mat4 view = glm::lookAt(cameraPos, cameraTarget, cameraUp);
		glm::mat4 projection = glm::perspective(glm::radians(45.0f), (float)SCREEN_WIDTH / (float)SCREEN_HEIGHT, 0.1f, 100.0f);

		glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
		glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

		// Setting light and view positions
		glUniform3f(lightPosLoc, lightX, lightY, lightZ);
		glUniform3f(viewPosLoc, cameraPos.x, cameraPos.y, cameraPos.z);

		// Lantern rendering
		glm::vec3 lanternPos = glm::vec3(0.5f, 6.0f, 2.0f);
		glm::vec3 lanternColor = glm::vec3(1.0f, 0.5f, 0.0f);

		int lanternPosLoc = glGetUniformLocation(shaderProgram, "lanternPos");
		int lanternColorLoc = glGetUniformLocation(shaderProgram, "lanternColor");

		glUniform3f(lanternPosLoc, lanternPos.x, lanternPos.y, lanternPos.z);
		glUniform3f(lanternColorLoc, 3 * lanternColor.x, 3 * lanternColor.y, 3 * lanternColor.z);

		// Lantern pole rendering
		glUniform1i(renderModeLoc, 1);
		glm::mat4 modelLanternPole = glm::mat4(1.0f);
		modelLanternPole = glm::translate(modelLanternPole, glm::vec3(0.5f, 2.0f, 2.0f));
		modelLanternPole = glm::scale(modelLanternPole, glm::vec3(0.2f, 8.0f, 0.2f));
		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelLanternPole));
		glBindVertexArray(cubeVAO);
		glDrawArrays(GL_TRIANGLES, 0, 36);

		// Lantern bulb rendering
		glUniform1i(renderModeLoc, 2);
		glUniform3f(objectColorLoc, lanternColor.x, lanternColor.y, lanternColor.z);
		glm::mat4 modelLanternBulb = glm::mat4(1.0f);
		modelLanternBulb = glm::translate(modelLanternBulb, lanternPos); 
		modelLanternBulb = glm::scale(modelLanternBulb, glm::vec3(0.4f)); 

		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelLanternBulb));
		glBindVertexArray(sphereVAO);
		glDrawElements(GL_TRIANGLES, (unsigned int)sphereIndices.size(), GL_UNSIGNED_INT, 0);

		// Floor rendering
		glUniform1i(renderModeLoc, 1);

		glm::mat4 modelFloor = glm::mat4(1.0f);
		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelFloor));

		glBindVertexArray(VAO_Floor);
		glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);

		// Torus rendering 
		glUniform1i(renderModeLoc, 1);
		glm::mat4 modelTorus = glm::mat4(1.0f);
		modelTorus = glm::translate(modelTorus, glm::vec3(3.0f, 1.0f, -3.0f));
		modelTorus = glm::rotate(modelTorus, glm::radians(30.0f), glm::vec3(1.0f, 0.0f, 0.0f));
		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelTorus));
		glBindVertexArray(torusVAO);
		glDrawElements(GL_TRIANGLES, (unsigned int)torusIndicesNew.size(), GL_UNSIGNED_INT, 0);
		
		// Light source rendering 
		glUniform1i(renderModeLoc, 2);
		glUniform3f(objectColorLoc, 1.0f, 1.0f, 0.5f);

		glm::mat4 modelLight = glm::mat4(1.0f);

		modelLight = glm::translate(modelLight, glm::vec3(lightX, lightY, lightZ));
		modelLight = glm::scale(modelLight, glm::vec3(1.5f));

		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelLight));

		glBindVertexArray(sphereVAO);
		glDrawElements(GL_TRIANGLES, (unsigned int)sphereIndices.size(), GL_UNSIGNED_INT, 0);

		// Tetroid rendering
		glUniform1i(renderModeLoc, 1);
		glm::mat4 modelTetra = glm::mat4(1.0f);
		modelTetra = glm::translate(modelTetra, glm::vec3(-2.0f, 1.0f, 0.5f));
		modelTetra = glm::rotate(modelTetra, glm::radians(30.0f), glm::vec3(1.0f, 0.0f, 0.0f));
		modelTetra = glm::rotate(modelTetra, glm::radians(45.0f), glm::vec3(0.0f, 1.0f, 0.0f));

		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelTetra));
		glBindVertexArray(tetraVAO);
		glDrawArrays(GL_TRIANGLES, 0, 12);

		// Sphere rendering
		glUniform1i(renderModeLoc, 1);
		glm::mat4 modelSphere = glm::mat4(1.0f);
		modelSphere = glm::translate(modelSphere, spherePos);
		modelSphere = glm::scale(modelSphere, glm::vec3(0.3f));
		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelSphere));
		glBindVertexArray(sphereVAO);
		glDrawElements(GL_TRIANGLES, (unsigned int)sphereIndices.size(), GL_UNSIGNED_INT, 0);
		


		glfwSwapBuffers(window);
		glfwPollEvents();
	}

	glDeleteVertexArrays(1, &VAO_Floor);
	glDeleteBuffers(1, &VBO_Floor);
	glDeleteBuffers(1, &EBO_Floor);

	glDeleteVertexArrays(1, &tetraVAO);
	glDeleteBuffers(1, &tetraVBO);

	glDeleteVertexArrays(1, &torusVAO);
	glDeleteBuffers(1, &torusVBO);
	glDeleteBuffers(1, &torusEBO);

	glDeleteVertexArrays(1, &sphereVAO);
	glDeleteBuffers(1, &sphereVBO);
	glDeleteBuffers(1, &sphereEBO);

	glDeleteProgram(shaderProgram);

	glfwTerminate();
	return 0;

}
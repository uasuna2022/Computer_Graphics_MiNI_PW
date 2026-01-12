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


	// Tetraid generation
	float vertices1[] = {
		// positions           // colors
		1.0f,  1.0f,  1.0f,    1.0f, 0.0f, 0.0f, // v0 - red
		-1.0f, -1.0f,  1.0f,    0.0f, 1.0f, 0.0f, // v1 - green
		-1.0f,  1.0f, -1.0f,    0.0f, 0.0f, 1.0f, // v2 - blue
		1.0f, -1.0f, -1.0f,    1.0f, 1.0f, 0.0f  // v3 - yellow
	};

	unsigned int indices1[] = {
					0, 1, 2,
					0, 3, 1,
					0, 2, 3,
					1, 3, 2
	};

	unsigned int VBO1, VAO1, EBO1;

	glGenVertexArrays(1, &VAO1);
	glGenBuffers(1, &VBO1);
	glGenBuffers(1, &EBO1);

	glBindVertexArray(VAO1);
	glBindBuffer(GL_ARRAY_BUFFER, VBO1);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices1), vertices1, GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO1);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(indices1), indices1, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);

	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);

	glBindVertexArray(0);

	
	// Torus1 generation 
	
	std::vector<float> torusVertices;
	std::vector<unsigned int> torusIndices;

	
	generateTorus(torusVertices, torusIndices, 1.0f, 0.2f, 50, 30);

	unsigned int VBO_Torus, VAO_Torus, EBO_Torus;
	glGenVertexArrays(1, &VAO_Torus);
	glGenBuffers(1, &VBO_Torus);
	glGenBuffers(1, &EBO_Torus);

	glBindVertexArray(VAO_Torus);

	glBindBuffer(GL_ARRAY_BUFFER, VBO_Torus);
	glBufferData(GL_ARRAY_BUFFER, torusVertices.size() * sizeof(float), torusVertices.data(), GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO_Torus);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, torusIndices.size() * sizeof(unsigned int), torusIndices.data(), GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	glBindVertexArray(0);

	// Torus2 generation 

	std::vector<float> torusVertices2;
	std::vector<unsigned int> torusIndices2;


	generateTorus(torusVertices2, torusIndices2, 1.0f, 0.2f, 50, 30);

	unsigned int VBO_Torus2, VAO_Torus2, EBO_Torus2;
	glGenVertexArrays(1, &VAO_Torus2);
	glGenBuffers(1, &VBO_Torus2);
	glGenBuffers(1, &EBO_Torus2);

	glBindVertexArray(VAO_Torus2);

	glBindBuffer(GL_ARRAY_BUFFER, VBO_Torus2);
	glBufferData(GL_ARRAY_BUFFER, torusVertices2.size() * sizeof(float), torusVertices2.data(), GL_STATIC_DRAW);

	glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO_Torus2);
	glBufferData(GL_ELEMENT_ARRAY_BUFFER, torusIndices2.size() * sizeof(unsigned int), torusIndices2.data(), GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)0);
	glEnableVertexAttribArray(0);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(3 * sizeof(float)));
	glEnableVertexAttribArray(1);
	glVertexAttribPointer(2, 3, GL_FLOAT, GL_FALSE, 9 * sizeof(float), (void*)(6 * sizeof(float)));
	glEnableVertexAttribArray(2);

	glBindVertexArray(0);


	while (!glfwWindowShouldClose(window))
	{
		glClearColor(0.2f, 0.3f, 0.3f, 1.0f);
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		glUseProgram(shaderProgram);

		int renderModeLoc = glGetUniformLocation(shaderProgram, "renderMode");
		int lightPosLoc = glGetUniformLocation(shaderProgram, "lightPos");
		int viewPosLoc = glGetUniformLocation(shaderProgram, "viewPos");

		int modelLoc = glGetUniformLocation(shaderProgram, "model");
		int viewLoc = glGetUniformLocation(shaderProgram, "view");
		int projLoc = glGetUniformLocation(shaderProgram, "projection");

		glm::mat4 view = glm::translate(glm::mat4(1.0f), glm::vec3(0.0f, 0.0f, -7.0f));
		glm::mat4 projection = glm::perspective(glm::radians(45.0f), (float)SCREEN_WIDTH / (float)SCREEN_HEIGHT, 0.1f, 100.0f);

		glUniformMatrix4fv(viewLoc, 1, GL_FALSE, glm::value_ptr(view));
		glUniformMatrix4fv(projLoc, 1, GL_FALSE, glm::value_ptr(projection));

		glUniform3f(lightPosLoc, 2.0f, 2.0f, 5.0f); 
		glUniform3f(viewPosLoc, 0.0f, 0.0f, 7.0f);    


		float time = (float)glfwGetTime();

		// Tetrahedron rendering
		glUniform1i(renderModeLoc, 0);

		glm::mat4 modelTetra = glm::mat4(1.0f);
		modelTetra = glm::rotate(modelTetra, time * 1.5f, glm::vec3(1.0f, 1.0f, 0.0f));
		modelTetra = glm::scale(modelTetra, glm::vec3(0.85f));

		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelTetra));

		glBindVertexArray(VAO1);
		glDrawElements(GL_TRIANGLES, 12, GL_UNSIGNED_INT, 0);


		// Torus1 rendering
		glUniform1i(renderModeLoc, 0);

		glm::mat4 modelTorus = glm::mat4(1.0f);
		modelTorus = glm::rotate(modelTorus, -time * 0.8f, glm::vec3(0.0f, 1.0f, 1.0f));

		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelTorus));

		glBindVertexArray(VAO_Torus);
		glDrawElements(GL_TRIANGLES, static_cast<unsigned int>(torusIndices.size()), GL_UNSIGNED_INT, 0);


		// Torus2 rendering (Phong shading)
		glUniform1i(renderModeLoc, 1); 

		glm::mat4 modelTorus2 = glm::mat4(1.0f);

		modelTorus2 = glm::translate(modelTorus2, glm::vec3(1.0f, 0.0f, 0.0f));

		modelTorus2 = glm::rotate(modelTorus2, glm::radians(60.0f), glm::vec3(1.0f, 0.0f, 0.0f));

		modelTorus2 = glm::rotate(modelTorus2, time * 1.0f, glm::vec3(0.0f, 1.0f, 0.0f));

		modelTorus2 = glm::scale(modelTorus2, glm::vec3(1.15f));

		glUniformMatrix4fv(modelLoc, 1, GL_FALSE, glm::value_ptr(modelTorus2));

		glBindVertexArray(VAO_Torus2);
		glDrawElements(GL_TRIANGLES, static_cast<unsigned int>(torusIndices2.size()), GL_UNSIGNED_INT, 0);


		glfwSwapBuffers(window);
		glfwPollEvents();
	}

	glDeleteVertexArrays(1, &VAO1);
	glDeleteBuffers(1, &VBO1);
	glDeleteBuffers(1, &EBO1);

	glDeleteVertexArrays(1, &VAO_Torus);
	glDeleteBuffers(1, &VBO_Torus);
	glDeleteBuffers(1, &EBO_Torus);

	glDeleteProgram(shaderProgram);

	glfwTerminate();
	return 0;

}
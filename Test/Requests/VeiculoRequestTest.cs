using System.Net;
using System.Text;
using System.Text.Json;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests
{
    [TestClass]
    public class VeiculoRequestTest
    {
        private static string token = string.Empty;

        [ClassInitialize]
        public static async Task ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);

            // Login para obter token
            var loginDTO = new LoginDTO
            {
                Email = "adm@teste.com", // usuário com papel Adm ou Editor
                Senha = "123456"
            };

            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
            var response = await Setup.client.PostAsync("/administradores/login", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            token = admLogado?.Token ?? "";
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task TestarCriarVeiculo()
        {
            // Arrange
            var veiculoDTO = new VeiculoDTO
            {
                Nome = "Carro Teste",
                Marca = "Marca X",
                Ano = 2022
            };

            var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
            Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await Setup.client.PostAsync("/veiculos", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculoCriado = JsonSerializer.Deserialize<VeiculoModelView>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(veiculoCriado);
            Assert.AreEqual(veiculoDTO.Nome, veiculoCriado?.Nome);
            Assert.AreEqual(veiculoDTO.Marca, veiculoCriado?.Marca);
            Assert.AreEqual(veiculoDTO.Ano, veiculoCriado?.Ano);

            Console.WriteLine($"Veículo criado: {veiculoCriado?.Nome} (ID: {veiculoCriado?.Id})");
        }

        [TestMethod]
        public async Task TestarGetVeiculos()
        {
            Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await Setup.client.GetAsync("/veiculos");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculos = JsonSerializer.Deserialize<List<VeiculoModelView>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(veiculos);
            Assert.IsTrue(veiculos.Count > 0);

            Console.WriteLine($"Total de veículos: {veiculos.Count}");
        }

        [TestMethod]
        public async Task TestarGetVeiculoPorId()
        {
            Setup.client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Primeiro criar um veículo para testar GET por ID
            var veiculoDTO = new VeiculoDTO
            {
                Nome = "Carro Para Buscar",
                Marca = "Marca Y",
                Ano = 2021
            };

            var content = new StringContent(JsonSerializer.Serialize(veiculoDTO), Encoding.UTF8, "application/json");
            var postResponse = await Setup.client.PostAsync("/veiculos", content);
            var postResult = await postResponse.Content.ReadAsStringAsync();
            var veiculoCriado = JsonSerializer.Deserialize<VeiculoModelView>(postResult, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Act
            var response = await Setup.client.GetAsync($"/veiculos/{veiculoCriado?.Id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();
            var veiculoBuscado = JsonSerializer.Deserialize<VeiculoModelView>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(veiculoBuscado);
            Assert.AreEqual(veiculoCriado?.Id, veiculoBuscado?.Id);
            Assert.AreEqual(veiculoDTO.Nome, veiculoBuscado?.Nome);

            Console.WriteLine($"Veículo buscado: {veiculoBuscado?.Nome} (ID: {veiculoBuscado?.Id})");
        }
    }
}

using Moq;
using ORBIT9000.Abstractions.Data;
using ORBIT9000.Core.Environment;
using ORBIT9000.Data;
using ORBIT9000.Data.Adapters;
using ORBIT9000.Data.Context;
using ORBIT9000.ExampleDomain.Entities;

namespace ORBIT9000.Engine.Tests
{
    [TestFixture]
    public partial class Repository : Disposable
    {
        private ReflectiveInMemoryContext reflectiveInMemoryContext;
        private ReflectiveInMemoryDbAdapter reflectiveInMemoryDbAdapter;
        private Repository<WeatherData> weatherDataRepository;

        [SetUp]
        public void Setup()
        {
            reflectiveInMemoryContext = new ReflectiveInMemoryContext();
            reflectiveInMemoryDbAdapter = new ReflectiveInMemoryDbAdapter(reflectiveInMemoryContext);
            weatherDataRepository = new Repository<WeatherData>(reflectiveInMemoryDbAdapter);
        }

        [TearDown]
        public void TearDown()
        {
            reflectiveInMemoryContext.Database.EnsureDeleted();
            reflectiveInMemoryContext.Dispose();
        }

        [Test]
        public void WeatherDataRepository_BasicOperationsTest()
        {
            Mock<IRepository<WeatherData>> weatherDataRepositoryMock = new();

            List<WeatherData> mockWeatherDataList =
            [
                new WeatherData { Id = Guid.NewGuid(), City = "Seattle", Temperature = 15.5m, Lattitude = 47.6062f, Longitude = -122.3321f },
                new WeatherData { Id = Guid.NewGuid(), City = "Portland", Temperature = 18.2m, Lattitude = 45.5152f, Longitude = -122.6784f },
                new WeatherData { Id = Guid.NewGuid(), City = "San Francisco", Temperature = 22.1m, Lattitude = 37.7749f, Longitude = -122.4194f }
            ];

            weatherDataRepositoryMock.Setup(repository => repository.GetAll()).Returns(mockWeatherDataList.AsQueryable());
            weatherDataRepositoryMock.Setup(repository => repository.FindById(It.IsAny<object[]>()))
                .Returns((object[] keys) => mockWeatherDataList.FirstOrDefault(weatherData => weatherData.Id.Equals(keys[0])));

            List<WeatherData> allWeatherDataFromMock = [.. weatherDataRepositoryMock.Object.GetAll()];

            Assert.That(allWeatherDataFromMock, Has.Count.EqualTo(3));
            Assert.That(allWeatherDataFromMock.Any(weatherData => weatherData.City == "Seattle"), Is.True);

            WeatherData expectedSeattleData = mockWeatherDataList[0];
            WeatherData? foundSeattleData = weatherDataRepositoryMock.Object.FindById(expectedSeattleData.Id);

            Assert.That(foundSeattleData, Is.Not.Null);
            Assert.That(foundSeattleData!.City, Is.EqualTo("Seattle"));

            WeatherData newChicagoWeatherData = new()
            {
                Id = Guid.NewGuid(),
                City = "Chicago",
                Temperature = 12.8m,
                Lattitude = 41.8781f,
                Longitude = -87.6298f
            };

            weatherDataRepositoryMock.Object.Add(newChicagoWeatherData);
            weatherDataRepositoryMock.Verify(repository => repository.Add(It.Is<WeatherData>(weatherData => weatherData.City == "Chicago")), Times.Once);

            weatherDataRepositoryMock.Object.Remove(expectedSeattleData);
            weatherDataRepositoryMock.Verify(repository => repository.Remove(It.Is<WeatherData>(weatherData => weatherData.City == "Seattle")), Times.Once);

            weatherDataRepositoryMock.Object.Save();
            weatherDataRepositoryMock.Verify(repository => repository.Save(), Times.Once);
        }

        [Test]
        public void WeatherDataRepository_IntegrationTest()
        {
            WeatherData seattleWeatherData = new() { Id = Guid.NewGuid(), City = "Seattle", Temperature = 15.5m, Lattitude = 47.6062f, Longitude = -122.3321f };
            WeatherData portlandWeatherData = new() { Id = Guid.NewGuid(), City = "Portland", Temperature = 18.2m, Lattitude = 45.5152f, Longitude = -122.6784f };
            WeatherData sanFranciscoWeatherData = new() { Id = Guid.NewGuid(), City = "San Francisco", Temperature = 22.1m, Lattitude = 37.7749f, Longitude = -122.4194f };

            weatherDataRepository.Add(seattleWeatherData);
            weatherDataRepository.Add(portlandWeatherData);
            weatherDataRepository.Add(sanFranciscoWeatherData);
            weatherDataRepository.Save();

            List<WeatherData> allStoredWeatherData = [.. weatherDataRepository.GetAll()];

            Assert.That(allStoredWeatherData, Has.Count.EqualTo(3));
            Assert.That(allStoredWeatherData.Any(weatherData => weatherData.City == "Seattle"), Is.True);

            WeatherData? foundSeattleData = weatherDataRepository.FindById(seattleWeatherData.Id);

            Assert.That(foundSeattleData, Is.Not.Null);
            Assert.That(foundSeattleData!.City, Is.EqualTo("Seattle"));

            seattleWeatherData.Temperature = 16.2m;
            weatherDataRepository.UpdateAsync(seattleWeatherData);
            weatherDataRepository.Save();

            foundSeattleData = weatherDataRepository.FindById(seattleWeatherData.Id);

            Assert.That(foundSeattleData!.Temperature, Is.EqualTo(16.2m));

            weatherDataRepository.Remove(seattleWeatherData);
            weatherDataRepository.Save();

            allStoredWeatherData = [.. weatherDataRepository.GetAll()];

            Assert.That(allStoredWeatherData, Has.Count.EqualTo(2));
            Assert.That(allStoredWeatherData.Any(weatherData => weatherData.City == "Seattle"), Is.False);
        }

        [Test]
        public void WeatherDataRepository_QueryFiltering_Test()
        {
            List<WeatherData> testWeatherDataList =
            [
                new WeatherData { Id = Guid.NewGuid(), City = "Seattle", Temperature = 15.5m, Lattitude = 47.6062f, Longitude = -122.3321f },
                new WeatherData { Id = Guid.NewGuid(), City = "Portland", Temperature = 18.2m, Lattitude = 45.5152f, Longitude = -122.6784f },
                new WeatherData { Id = Guid.NewGuid(), City = "Seattle", Temperature = 10.0m, Lattitude = 47.6062f, Longitude = -122.3321f },
                new WeatherData { Id = Guid.NewGuid(), City = "San Francisco", Temperature = 22.1m, Lattitude = 37.7749f, Longitude = -122.4194f }
            ];

            foreach (WeatherData weatherData in testWeatherDataList)
            {
                weatherDataRepository.Add(weatherData);
            }

            weatherDataRepository.Save();

            List<WeatherData> seattleWeatherDataList
                = [.. weatherDataRepository.GetAll().Where(weatherData => weatherData.City == "Seattle")];

            Assert.That(seattleWeatherDataList, Has.Count.EqualTo(2));
            Assert.That(seattleWeatherDataList.All(weatherData => weatherData.City == "Seattle"), Is.True);

            List<WeatherData> coolWeatherDataList
                = [.. weatherDataRepository.GetAll().Where(weatherData => weatherData.Temperature < 15.0m)];

            Assert.That(coolWeatherDataList, Has.Count.EqualTo(1));
            Assert.That(coolWeatherDataList[0].Temperature, Is.EqualTo(10.0m));

            List<WeatherData> filteredSeattleWarmWeatherDataList
                = [.. weatherDataRepository.GetAll().Where(weatherData => weatherData.City == "Seattle" && weatherData.Temperature > 12.0m)];

            Assert.That(filteredSeattleWarmWeatherDataList, Has.Count.EqualTo(1));
            Assert.That(filteredSeattleWarmWeatherDataList[0].Temperature, Is.EqualTo(15.5m));
        }

        protected override void DisposeManagedObjects()
        {
            reflectiveInMemoryContext?.Dispose();
            base.DisposeManagedObjects();
        }
    }
}
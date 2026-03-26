using System.Data;

namespace WeatherApi.Application.Common.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

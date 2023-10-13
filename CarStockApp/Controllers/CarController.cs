using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CarStockApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarController : ControllerBase
{
    
    //Data structure storing car information
    static Dictionary<string, List<Car>> carDict = new Dictionary<string, List<Car>>();

    /**
     * search cars based on dealer, model, make
     * if model and make is empty, then list all cars by default
     */
    [HttpPost]
    [Route("list")]
    public IActionResult GetCars(CarQuery carQuery)
    {
        string dealer = carQuery.Dealer;
        string make = carQuery.Make;
        string model = carQuery.Model;
        
        if (carDict.ContainsKey(dealer))
        {
            List<Car> cars = carDict[dealer];
            
            if (!String.IsNullOrEmpty(make))
            {
                Regex regex = new Regex("^.*" + make + ".*$");
                cars = cars.Where(car => regex.IsMatch(car.Make)).ToList();
            }
            
            if (!String.IsNullOrEmpty(model))
            {
                Regex regex = new Regex("^.*" + model + ".*$");
                cars = cars.Where(car => regex.IsMatch(car.Model)).ToList();
            }

            return Ok(cars);
        }
        return NotFound("Not Found");
    }
    
    /**
     * insert a new car
     */
    [HttpPost]
    [Route("insert")]
    public IActionResult AddCars(AddCarRequest addCarRequest)
    {
        string dealer = addCarRequest.Dealer;

        Car car = new Car()
        {
            Id = Guid.NewGuid().ToString(),
            Make = addCarRequest.Make,
            Model = addCarRequest.Model,
            Year = addCarRequest.Year,
            Stock = 0
        };

        if (carDict.ContainsKey(dealer))
        {
            List<Car> cars = carDict[dealer];
            cars.Add(car);
        }
        else
        {
            List<Car> cars = new List<Car>();
            cars.Add(car);
            carDict.Add(dealer, cars);
        }

        return Ok("Add Success");
        
    }

    /**
     * delete a car based on dealer and car id
     */
    [HttpDelete]
    [Route("delete")]
    public IActionResult RemoveCars(RemoveCarRequest removeCarRequest)
    {
        string dealer = removeCarRequest.Dealer;
        string id = removeCarRequest.Id;

        if (carDict.ContainsKey(dealer))
        {
            int removePosition = -1;
            List<Car> cars = carDict[dealer];
            for (int i = 0; i < cars.Count; i++)
            {
                Car car = cars[i];
                if (car.Id.Equals(id))
                {
                    removePosition = i;
                    break;
                }
            }

            if (removePosition != -1)
            {
                cars.RemoveAt(removePosition);
                return Ok("Remove Success");
            }
            
            return NotFound("ID Not Found");
            
        }

        return NotFound("Dealer Not Exist");
    }
    
    /**
     * update car stock based on dealer and car Id
     */
    [HttpPut]
    [Route("update")]
    public IActionResult UpdateCars(CarUpdateRequest carUpdateRequest)
    {
        
        string dealer = carUpdateRequest.Dealer;
        string id = carUpdateRequest.Id;
        
        if (carDict.ContainsKey(dealer))
        {
            List<Car> cars = carDict[dealer];
            for (int i = 0; i < cars.Count; i++)
            {
                Car car = cars[i];
                if (car.Id.Equals(id))
                {
                    car.Stock = carUpdateRequest.Stock;
                    return Ok("Update Success");
                }
            }

            return NotFound("ID Not Exist");
        }

        return NotFound("Dealer Not Exist");
    }

}
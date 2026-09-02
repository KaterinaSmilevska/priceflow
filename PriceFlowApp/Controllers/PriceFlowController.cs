using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Controllers
{
    public abstract class PriceFlowController : ControllerBase
    {
        protected ActionResult<T> Execute<T>(Func<T> action)
        {
            try
            {
                return Ok(action());
            }
            catch (PriceFlowException ex)
            {
                return StatusCode(
                    ex.StatusCode,
                    new ErrorResponse(ex.Code, ex.Message)
                    );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ErrorResponse(
                        "INTERNAL_SERVER_ERROR",
                        "An unexpected error occured.")
                    );
            }
        }

        protected async Task<ActionResult<T>> ExecuteAsync<T>(Func<Task<T>> action)
        {
            try
            {
                return Ok(await action());
            }
            catch (PriceFlowException ex)
            {
                return StatusCode(
                    ex.StatusCode,
                    new ErrorResponse(ex.Code, ex.Message)
                    );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ErrorResponse(
                        "INTERNAL_SERVER_ERROR",
                        "An unexpected error occured.")
                    );
            }
        }

        protected IActionResult Execute(Action action)
        {
            try
            {
                action();
                return Ok();
            }
            catch (PriceFlowException ex)
            {
                return StatusCode(
                    ex.StatusCode,
                    new ErrorResponse(ex.Code, ex.Message)
                    );
            }
            catch (Exception)
            {
                return StatusCode(
                    500,
                    new ErrorResponse(
                        "INTERNAL_SERVER_ERROR",
                        "An unexpected error occured.")
                    );
            }
        }
    }
}

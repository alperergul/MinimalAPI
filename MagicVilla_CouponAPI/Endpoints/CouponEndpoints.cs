using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_CouponAPI.Endpoints
{
    public static class CouponEndpoints
    {
        public static void ConfigureCouponEndpoints(this WebApplication app)
        {

            app.MapGet("/api/coupon", GetAllCoupon).WithName("GetCoupons").Produces<APIResponse>(200);

            app.MapGet("/api/coupon/{id:int}", GetCouponById).WithName("GetCoupon").Produces<APIResponse>(200);

            app.MapPost("/api/coupon", CreateCoupon).Accepts<CouponCreateDTO>("application/json")
            .WithName("CreateCoupon")
            .Produces<APIResponse>(201)
            .Produces(400);

            app.MapPut("/api/coupon", UpdateCoupon).WithName("UpdateCoupon")
            .Accepts<CouponUpdateDTO>("application/json")
            .Produces<APIResponse>(200).Produces(400);

            app.MapDelete("/api/coupon/{id:int}", DeleteCoupon);
        }


        private async static Task<IResult> GetAllCoupon(ICouponRepository _couponRepository, ILogger<Program> _logger)
        {
            APIResponse response = new();

            _logger.Log(LogLevel.Information, "Getting all coupons");

            response.Result = await _couponRepository.GetAllAsync();
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }

        private async static Task<IResult> GetCouponById(ICouponRepository _couponRepository, ILogger<Program> _logger, int id)
        {
            APIResponse response = new();

            _logger.Log(LogLevel.Information, $"Get {id} coupon");

            response.Result = await _couponRepository.GetAsync(id);
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;

            return Results.Ok(response);
        }

        private async static Task<IResult> CreateCoupon(ICouponRepository _couponRepository, IMapper _mappper, IValidator<CouponCreateDTO> _validation, [FromBody] CouponCreateDTO coupon_C_DTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var validationResult = await _validation.ValidateAsync(coupon_C_DTO);


            if (!validationResult.IsValid)
            {
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());

                return Results.BadRequest(response);
            }

            if (await _couponRepository.GetAsync(coupon_C_DTO.Name) != null)
            {
                response.ErrorMessages.Add("Coupon Name already exists");
                return Results.BadRequest(response);
            }

            Coupon coupon = _mappper.Map<Coupon>(coupon_C_DTO);

            await _couponRepository.CreateAsync(coupon);
            await _couponRepository.SaveAsync();

            CouponDTO couponDTO = _mappper.Map<CouponDTO>(coupon);


            response.Result = couponDTO;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            return Results.Ok(response);

            //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
            //return Results.Created($"/api/coupon/{coupon.Id}", coupon);
        }

        private async static Task<IResult> UpdateCoupon(ICouponRepository _couponRepository, IMapper _mapper, IValidator<CouponUpdateDTO> _validation, [FromBody] CouponUpdateDTO coupon_U_DTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            var validationResult = await _validation.ValidateAsync(coupon_U_DTO);


            if (!validationResult.IsValid)
            {
                response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());

                return Results.BadRequest(response);
            }

            await _couponRepository.UpdateAsync(_mapper.Map<Coupon>(coupon_U_DTO));
            await _couponRepository.SaveAsync();

            response.Result = _mapper.Map<CouponDTO>(await _couponRepository.GetAsync(coupon_U_DTO.Id));
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }


        private async static Task<IResult> DeleteCoupon(ICouponRepository _couponRepository, int id)
        {

            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            Coupon couponFromStore = await _couponRepository.GetAsync(id);

            if (couponFromStore != null)
            {
                await _couponRepository.RemoveAsync(couponFromStore);
                await _couponRepository.SaveAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return Results.Ok(response);
            }
            else
            {
                response.ErrorMessages.Add("Invalid Id");
                return Results.BadRequest(response);
            }
        }

    }
}

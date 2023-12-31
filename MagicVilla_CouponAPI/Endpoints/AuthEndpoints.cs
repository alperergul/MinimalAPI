﻿using AutoMapper;
using FluentValidation;
using MagicVilla_CouponAPI.Models.DTO;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_CouponAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void ConfigureAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/login", Login)
                .WithName("Login")
                .Accepts<LoginRequestDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400);

            app.MapPost("/api/register", Register)
                .WithName("Register")
                .Accepts<RegisterationRequestDTO>("application/json")
                .Produces<APIResponse>(200)
                .Produces(400);
        }



        private async static Task<IResult> Login(IAuthRepository _authRepository, IMapper _mappper, [FromBody] LoginRequestDTO model)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            var loginResponse = await _authRepository.Login(model);

            if(loginResponse == null)
            {
                response.ErrorMessages.Add("Username or password is incorrect");
                return Results.BadRequest(response);
            }

            response.Result = loginResponse;
            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

        private async static Task<IResult> Register(IAuthRepository _authRepository, IMapper _mappper, [FromBody] RegisterationRequestDTO model)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

            bool ifUserNameisUnique = _authRepository.IsUniqueUser(model.UserName);

            if (!ifUserNameisUnique)
            {
                response.ErrorMessages.Add("Username already exists");
                return Results.BadRequest(response);
            }

            var registerResponse = await _authRepository.Register(model);

            if (registerResponse == null || string.IsNullOrEmpty(registerResponse.UserName))
            {
                return Results.BadRequest(response);
            }

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            return Results.Ok(response);
        }

    }
}

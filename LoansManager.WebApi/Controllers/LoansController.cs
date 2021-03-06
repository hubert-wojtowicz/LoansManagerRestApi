﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LoansManager.BussinesLogic.Commands.Models;
using LoansManager.BussinesLogic.Infrastructure.CommandsSetup;
using LoansManager.BussinesLogic.Infrastructure.SettingsModels;
using LoansManager.BussinesLogic.Interfaces;
using LoansManager.Common.Services;
using LoansManager.WebApi.Controllers.Models.LoansController;
using LoansManager.WebApi.Resources;
using Microsoft.AspNetCore.Mvc;

namespace LoansManager.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ApplicationBaseController
    {
        private readonly IMapper _mapper;
        private readonly ICommandBus _commandBus;
        private readonly ILoansService _loansService;
        private readonly IHttpContextService _uriHelperService;
        private readonly ApiSettings _apiSettings;

        public LoansController(
            IMapper mapper,
            ICommandBus commandBus,
            ILoansService loansService,
            IHttpContextService uriHelperService,
            ApiSettings apiSettings)
        {
            _mapper = mapper;
            _commandBus = commandBus;
            _loansService = loansService;
            _uriHelperService = uriHelperService;
            _apiSettings = apiSettings;
        }

        /// <summary>
        /// Gets loan by its <paramref name="id"/> key.
        /// </summary>
        /// <param name="id">Key of concrete loan.</param>
        /// <returns>Loan object.</returns>
        /// <response code="200">When loan found.</response>
        /// <response code="404">When no loan found.</response>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(GetLoanResponseModel))]
        [ProducesResponseType(404, Type = typeof(Guid))]
        public async Task<IActionResult> Get(Guid id)
        {
            var loan = await _loansService.Get(id);

            if (loan != null)
            {
                return Ok(_mapper.Map<GetLoanResponseModel>(loan));
            }

            return NotFound(id);
        }

        /// <summary>
        /// Gets collection of loans.
        /// </summary>
        /// <param name="offset">Offset from first record.</param>
        /// <param name="take">Amount of records to take.</param>
        /// <returns>Collection of loans objects.</returns>
        /// <response code="200">When at least one record found.</response>
        /// <response code="400">When to many records requested.</response>
        /// <response code="404">When no records found.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromQuery(Name = "offset")] int offset = 0, [FromQuery(Name = "take")] int take = 15)
        {
            if (take > _apiSettings.MaxNumberOfRecordToGet)
            {
                return BadRequest(ValidationResultFactory(nameof(take), take, UserControllerResources.MaxNumberOfRecordToGetExceeded, _apiSettings.MaxNumberOfRecordToGet.ToString(CultureInfo.InvariantCulture)));
            }

            var loans = await _loansService.Get(offset, take);
            if (loans.Any())
            {
                return Ok(loans);
            }

            return NotFound();
        }

        /// <summary>
        /// Gets list of cash borrowers.
        /// </summary>
        /// <param name="offset">Offset from first record.</param>
        /// <param name="take">Amount of records to take.</param>
        /// <returns>Collection of borrowers ids.</returns>
        /// <response code="200">When at least one record found.</response>
        /// <response code="400">When to many records requested.</response>
        /// <response code="404">When no records found.</response>
        [HttpGet]
        [Route("Borrowers")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBorrowers([FromQuery(Name = "offset")] int offset = 0, [FromQuery(Name = "take")] int take = 15)
        {
            if (take > _apiSettings.MaxNumberOfRecordToGet)
            {
                return BadRequest(ValidationResultFactory(nameof(take), take, UserControllerResources.MaxNumberOfRecordToGetExceeded, _apiSettings.MaxNumberOfRecordToGet.ToString(CultureInfo.InvariantCulture)));
            }

            var users = await _loansService.GetBorrowers(offset, take);
            if (users.Any())
            {
                return Ok(users);
            }

            return NotFound();
        }

        /// <summary>
        /// Gets list of cash lenders.
        /// </summary>
        /// <param name="offset">Offset from first record.</param>
        /// <param name="take">Amount of records to take.</param>
        /// <returns>Collection of lenders ids.</returns>
        /// <response code="200">When at least one record found.</response>
        /// <response code="400">When to many records requested.</response>
        /// <response code="404">When no records found.</response>
        [HttpGet]
        [Route("Lenders")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLenders([FromQuery(Name = "offset")] int offset = 0, [FromQuery(Name = "take")] int take = 15)
        {
            if (take > _apiSettings.MaxNumberOfRecordToGet)
            {
                return BadRequest(ValidationResultFactory(nameof(take), take, UserControllerResources.MaxNumberOfRecordToGetExceeded, _apiSettings.MaxNumberOfRecordToGet.ToString(CultureInfo.InvariantCulture)));
            }

            var users = await _loansService.GetLenders(offset, take);
            if (users.Any())
            {
                return Ok(users);
            }

            return NotFound();
        }

        /// <summary>
        /// Gets collection of loans to pay off by specified user.
        /// </summary>
        /// <param name="userId">User who borrowed cash.</param>
        /// <param name="offset">Offset from first record.</param>
        /// <param name="take">Amount of records to take.</param>
        /// <returns>Collection of loans.</returns>
        /// <response code="200">When at least one record found.</response>
        /// <response code="400">When to many records requested.</response>
        /// <response code="404">When no records found.</response>
        [HttpGet]
        [Route("{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserLoans(string userId, [FromQuery(Name = "offset")] int offset = 0, [FromQuery(Name = "take")] int take = 15)
        {
            if (take > _apiSettings.MaxNumberOfRecordToGet)
            {
                return BadRequest(ValidationResultFactory(nameof(take), take, UserControllerResources.MaxNumberOfRecordToGetExceeded, _apiSettings.MaxNumberOfRecordToGet.ToString(CultureInfo.InvariantCulture)));
            }

            var loans = await _loansService.GetUserLoans(userId, offset, take);
            if (loans.Any())
            {
                return Ok(loans);
            }

            return NotFound();
        }

        /// <summary>
        /// Adds new loan defined by model.
        /// </summary>
        /// <param name="createLoanCommand">Loan model.</param>
        /// <response code="201">When loan created.</response>
        /// <response code="400">When validation on <paramref name="createLoanCommand"/> failed.</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody]CreateLoanCommand createLoanCommand)
        {
            var validationResult = await _commandBus.Validate(createLoanCommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            await _commandBus.Submit(createLoanCommand);
#pragma warning disable CA1062 // Validate arguments of public methods
            return Created(_uriHelperService.GetApiUrl($"api/users/{createLoanCommand.Id}"), createLoanCommand);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <summary>
        /// Repays specified loan by model.
        /// </summary>
        /// <param name="repayLoanCommand">Loan model.</param>
        /// <response code="202">When accepted loan repaid.</response>
        /// <response code="400">When validation on <paramref name="repayLoanCommand"/> failed.</response>
        [HttpPatch]
        [Route("Repay")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Repay([FromBody]RepayLoanCommand repayLoanCommand)
        {
            var validationResult = await _commandBus.Validate(repayLoanCommand);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult);
            }

            await _commandBus.Submit(repayLoanCommand);
            return Accepted(repayLoanCommand);
        }
    }
}

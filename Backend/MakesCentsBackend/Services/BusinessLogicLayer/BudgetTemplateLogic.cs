/*
 * Gianna Ross
 * Makes Cents
 * Sources: 
 */
using MakesCentsBackend.Models;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class BudgetTemplateLogic
    {
        // Class level variables
        private readonly BudgetTemplateDAO _budgetTemplateDAO;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="budgetTemplateDAO"></param>
        public BudgetTemplateLogic(BudgetTemplateDAO budgetTemplateDAO)
        {
            _budgetTemplateDAO = budgetTemplateDAO;
        }

        /// <summary>
        /// Logic method to create a budget template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> CreateBudgetTemplateAsync(CreateBudgetTemplateRequest template)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Make sure the necessary information was sent
            if (template.BudgetId == 0 || template.UserId == 0 || string.IsNullOrEmpty(template.TemplateName))
            {
                return new BaseIdResponse(400, "Missing information for template creation");
            }
            // Call the DAO method
            response = await _budgetTemplateDAO.CreateBudgetTemplateAsync(template);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get every template for a budget
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetAllBudgetTemplatesResponse> GetAllBudgetTemplatesAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetAllBudgetTemplatesResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetAllBudgetTemplatesResponse(400, "Missing information to get all templates");
            }
            // Call the DAO method
            response = await _budgetTemplateDAO.GetAllBudgetTemplatesAsync(request);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to get a single budget template, including its envelope entries
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<GetBudgetTemplateResponse> GetBudgetTemplateAsync(BaseIdRequest request)
        {
            // Declare and initialize
            GetBudgetTemplateResponse response;

            // Make sure the required information was sent
            if (request.EntityId == 0 || request.UserId == 0)
            {
                // Return the fail
                return new GetBudgetTemplateResponse(400, "Missing information to get template");
            }
            // Call the DAO method
            response = await _budgetTemplateDAO.GetBudgetTemplateAsync(request);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to rename a budget template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<BaseIdResponse> UpdateBudgetTemplateAsync(EditBudgetTemplateRequest template)
        {
            // Declare and initialize
            BaseIdResponse response;

            // Check to make sure the required information was provided
            if (template.BudgetTemplateId == 0 || template.UserId == 0)
            {
                return new BaseIdResponse(400, "Missing information for update");
            }
            // Call the DAO method
            response = await _budgetTemplateDAO.UpdateBudgetTemplateAsync(template);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to delete a budget template
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteBudgetTemplateAsync(BaseIdRequest request)
        {
            // Check to make sure the required information was provided
            if (request.EntityId == 0 || request.UserId == 0)
            {
                return new BaseResponse(400, "Missing information for delete");
            }
            // Return a call to the DAO method
            return await _budgetTemplateDAO.DeleteBudgetTemplateAsync(request);
        }

        /// <summary>
        /// Logic method to set a template's envelope entries
        /// </summary>
        /// <param name="templateEnvelopes"></param>
        /// <returns></returns>
        public async Task<GetBudgetTemplateResponse> SetBudgetTemplateEnvelopesAsync(SetBudgetTemplateEnvelopesRequest templateEnvelopes)
        {
            // Declare and initialize
            GetBudgetTemplateResponse response;

            // Make sure the necessary information was sent
            if (templateEnvelopes.BudgetTemplateId == 0 || templateEnvelopes.UserId == 0)
            {
                return new GetBudgetTemplateResponse(400, "Missing information to set the template's envelopes");
            }
            // Loop through the items to make sure the necessary information was sent for each
            foreach (SetBudgetTemplateEnvelopeItemRequest item in templateEnvelopes.Envelopes)
            {
                if (item == null)
                    return new GetBudgetTemplateResponse(400, "Envelope entry cannot be null");

                if (item.EnvelopeId == 0)
                    return new GetBudgetTemplateResponse(400, "Envelope entry's envelope is required");

                if (item.PlannedAmount <= 0)
                    return new GetBudgetTemplateResponse(400, "Envelope entry's planned amount must be greater than 0");
            }
            // Call the DAO method
            response = await _budgetTemplateDAO.SetBudgetTemplateEnvelopesAsync(templateEnvelopes);
            // Return the response
            return response;
        }

        /// <summary>
        /// Logic method to apply a template to a budget month, fully replacing that month's
        /// funding queue with the template's entries
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        public async Task<GetEnvelopeFundingQueueResponse> ApplyBudgetTemplateAsync(ApplyBudgetTemplateRequest apply)
        {
            // Declare and initialize
            GetEnvelopeFundingQueueResponse response;

            // Make sure the necessary information was sent
            if (apply.BudgetTemplateId == 0 || apply.BudgetMonthId == 0 || apply.UserId == 0)
            {
                return new GetEnvelopeFundingQueueResponse(400, "Missing information to apply the template");
            }
            // Call the DAO method
            response = await _budgetTemplateDAO.ApplyBudgetTemplateAsync(apply);
            // Return the response
            return response;
        }
    }
}
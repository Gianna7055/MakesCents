/*
 * Gianna Ross
 * File Created: 1/23/2026
 * File Last Updated: 1/23/2026
 * Makes Cents - Budget Logic
 * Sources: 
 */
using AutoMapper;
using MakesCentsBackend.Services.DataAccessLayer;

namespace MakesCentsBackend.Services.BusinessLogicLayer
{
    public class BudgetLogic
    {
        // Class level variables
        private BudgetDAO _budgetDAO;
        private IMapper _mapper;

        /// <summary>
        /// Parameterized constructor to bring in DI variables
        /// </summary>
        /// <param name="budgetDAO"></param>
        /// <param name="mapper"></param>
        public BudgetLogic(BudgetDAO budgetDAO, IMapper mapper)
        {
            _budgetDAO = budgetDAO;
            _mapper = mapper;
        }
    }
}

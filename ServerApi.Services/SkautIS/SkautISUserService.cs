using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Models;
using Models.DTO;
using ServerApi.Services.Base;

namespace ServerApi.Services.SkautIS
{
    public class SkautIsUserService : SkautIsService
    {
        public async Task<UserDetails> GetUserDetailsAsync(Guid loginId)
        {
            UserDetailsParsed userDetailsParsed;
            MembershipCategoryId membershipCategoryId;
            bool isUnitAdministrator;

            using (var userDetailsReponse = await GetUserDetailsAsync(loginId.ToString()))
            {
                if (!userDetailsReponse.IsSuccessStatusCode)
                    return null;

                userDetailsParsed = ParseUserDetailsResponseXmlAsync(await GetResponseXmlAsync(userDetailsReponse));
            }

            using (var userRolesResponse = await GetAllowedRoles(loginId.ToString(), userDetailsParsed.UserId))
            {
                if (!userRolesResponse.IsSuccessStatusCode)
                    return null;

                isUnitAdministrator = UserRolesContainUnitAdminAsync(await GetResponseXmlAsync(userRolesResponse));
            }

            using (var userMembershipResponse = await GetActiveMembership(loginId.ToString(), userDetailsParsed.PersonId, userDetailsParsed.UnitId))
            {
                if (!userMembershipResponse.IsSuccessStatusCode)
                    return null;

                membershipCategoryId = GetMembershipCategoryId(await GetResponseXmlAsync(userMembershipResponse));
            }

            return new UserDetails(int.Parse(userDetailsParsed.UserId), int.Parse(userDetailsParsed.UnitId), int.Parse(userDetailsParsed.PersonId),
                userDetailsParsed.UserName, isUnitAdministrator, membershipCategoryId);
        }

        internal async Task<HttpResponseMessage> GetUserDetailsAsync(string loginId)
        {
            var requestTemplate = XDocument.Parse(Properties.Resources.LoginDetailsRequestTemplate);
            
            var requestData = new Dictionary<string, string>
            {
                { "ID_Login", loginId },
            };

            return await Client.PostSoapRequest("UserManagement", "LoginDetail", requestTemplate, requestData);
        }

        internal async Task<HttpResponseMessage> GetAllowedRoles(string loginId, string userId)
        {
            var requestTemplate = XDocument.Parse(Properties.Resources.UserRoleAllRequestTemplate);
            var requestData = new Dictionary<string, string>
            {
                { "ID_Login", loginId },
                { "ID_User", userId },
            };

            return await Client.PostSoapRequest("UserManagement", "UserRoleAll", requestTemplate, requestData);
        }

        internal async Task<HttpResponseMessage> GetActiveMembership(string loginId, string personId, string unitId)
        {
            var requestTemplate = XDocument.Parse(Properties.Resources.MembershipAllPersonRequestTemplate);
            var requestData = new Dictionary<string, string>
            {
                { "ID_Login", loginId },
                { "ID_Person", personId },
                { "ID_Unit", unitId },
            };

            return await Client.PostSoapRequest("OrganizationUnit", "MembershipAllPerson", requestTemplate, requestData);
        }

        private bool UserRolesContainUnitAdminAsync(XDocument responseXml)
        {
            var adminRoleId = 18;
            return responseXml.Descendants(XName.Get("ID_Role", @"https://is.skaut.cz/")).Any(element => element.Value == adminRoleId.ToString());
        }

        private MembershipCategoryId GetMembershipCategoryId(XDocument responseXml)
        {
            return ConvertMembershipCategoryId(responseXml.Descendants(XName.Get("ID_MembershipCategory", @"https://is.skaut.cz/")).First().Value);
        }

        private UserDetailsParsed ParseUserDetailsResponseXmlAsync(
            XDocument responseXml)
        {
            var userId = responseXml.Descendants(XName.Get("ID_User", @"https://is.skaut.cz/")).First().Value;
            var personId = responseXml.Descendants(XName.Get("ID_Person", @"https://is.skaut.cz/")).First().Value;
            var unitId = responseXml.Descendants(XName.Get("ID_Unit", @"https://is.skaut.cz/")).First().Value;
            var userName = responseXml.Descendants(XName.Get("Person", @"https://is.skaut.cz/")).First().Value;

            return new UserDetailsParsed(userId, personId, unitId, userName);
        }

        private MembershipCategoryId ConvertMembershipCategoryId(string categoryId)
        {
            switch (categoryId)
            {
                case "ranger":
                case "rover":
                    return MembershipCategoryId.Guide;

                case "vlce":
                case "svetluska":
                    return MembershipCategoryId.Cub;

                case "skaut":
                case "skautka":
                    return MembershipCategoryId.Scout;

                default:
                    return MembershipCategoryId.Other;
            }
        }


        private struct UserDetailsParsed
        {
            public string UserId { get; }
            public string PersonId { get; }
            public string UnitId { get; }
            public string UserName { get; }

            public UserDetailsParsed(string userId, string personId, string unitId, string userName)
            {
                UserId = userId;
                PersonId = personId;
                UnitId = unitId;
                UserName = userName;
            }
        }
    }
}


# 🚀 Pull Request — .NET Enterprise Architecture Template

Thank you for contributing!  
Please complete the sections below to help maintainers review your PR efficiently and ensure architectural consistency.



## 🔍 Summary of Changes

**Describe clearly what this PR introduces:**

- What feature, fix, or improvement?
- Why is it needed?
- Any related issue?

> _Example: Implements the CreateProductCommand with validation, mapping, handler, and domain events._



## 🧩 Type of Change

Select all that apply:

- [ ] ✨ Feature (Command/Query/Module)
- [ ] 🐞 Bug Fix
- [ ] 📚 Documentation Update
- [ ] 🧹 Refactor / Cleanup
- [ ] 🏗 Architectural change
- [ ] 🔧 Infrastructure / Config
- [ ] 🧪 Tests (unit or integration)
- [ ] Other (specify):



## 📐 Architectural Considerations

**Explain how your PR respects the architectural rules** (Clean Architecture, CQRS, DDD):

- [ ] Follows the Dependency Rule (Domain has no external deps)
- [ ] CQRS separation respected (Commands vs Queries)
- [ ] Uses Pipeline Behaviors (Validation, Logging, Transactions)
- [ ] No business logic inside Controllers
- [ ] Domain Model enriched (Entities/VO/Aggregates/Events)
- [ ] Infrastructure code isolated
- [ ] Mapping profiles added/updated
- [ ] Validation rules included (FluentValidation)

> _If not applicable, explain why._



## 🧪 Testing

Describe the testing performed:

- [ ] Unit tests added
- [ ] Integration tests added
- [ ] All tests pass (`dotnet test`)
- [ ] Manual API testing via Swagger/Postman

Provide any logs, screenshots, or outputs if useful.



## 📚 Documentation Updates

This PR updates documentation in:

- [ ] `docs/architecture/`
- [ ] `docs/domain/`
- [ ] `docs/api/`
- [ ] `docs/setup/`
- [ ] Not required

If required but missing, the PR **cannot be merged**.



## 📝 Breaking Changes

Does this PR introduce breaking changes?

- [ ] Yes (explain)
- [ ] No



## 🔗 Related Issues

Link any related issues:

```

Closes #ISSUE_ID
Fixes #ISSUE_ID
Related to #ISSUE_ID

```



## ✔ Checklist Before Submitting

- [ ] Code compiles successfully  
- [ ] Follows the repository coding standards  
- [ ] Matches architecture conventions  
- [ ] Includes tests  
- [ ] Includes documentation  
- [ ] Includes meaningful commit messages (Conventional Commits)  
- [ ] I have reviewed the PR description  
- [ ] I agree to the Code of Conduct  



## 🙌 Additional Notes

Anything else reviewers should know?

> _If your PR introduces a major architectural component (e.g., new Aggregate, new Module, new Pipeline Behavior), please justify your approach clearly._



**Thank you for your contribution!**  
Your work helps build a world-class, enterprise-grade .NET architecture foundation.
 
---

## About the Author

**Stefano Giacchi**
Cloud Architect • Enterprise Architect Track • AI-Driven Engineer

Architecting large-scale distributed systems for over 20 years, with a focus on
**cloud-native platforms, enterprise integration patterns, AI-first architectures, and modern application design**.

Experienced across **Energy, Manufacturing, Finance, and Public Sector**, with a consistent track record of transforming complex requirements into scalable, maintainable and future-proof solutions.

Creator of advanced architectural accelerators and knowledge frameworks adopted by teams and practitioners across different countries.

Passionate about **AI governance, semantic architectures, intelligent APIs, and the evolution of enterprise software in the age of LLMs**.

### Certifications

[![Azure Architect Expert](https://img.shields.io/badge/Azure_Architect_Expert-0078D4?style=flat-square\&logo=microsoftazure\&logoColor=white)](#)
[![DevOps Engineer Expert](https://img.shields.io/badge/DevOps_Engineer_Expert-0078D4?style=flat-square\&logo=azurepipelines\&logoColor=white)](#)
[![ITIL 4](https://img.shields.io/badge/ITIL_4_Foundation-5C2D91?style=flat-square)](#)
[![PSM I](https://img.shields.io/badge/PSM_I-009FDA?style=flat-square)](#)
[![Neo4j Professional](https://img.shields.io/badge/Neo4j_Professional-008CC1?style=flat-square\&logo=neo4j\&logoColor=white)](#)
![C|FA](https://img.shields.io/badge/Cyber_Forensics_Associate-C%7CFA-critical?style=flat-square)

### Links

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Stefano_Giacchi-0A66C2?style=flat-square\&logo=linkedin)](https://www.linkedin.com/in/stefanogiacchi/)
[![GitHub](https://img.shields.io/badge/GitHub-stefanogiacchi-181717?style=flat-square\&logo=github)](https://github.com/stefanogiacchi)
[![HackerRank](https://img.shields.io/badge/HackerRank-Stefano_Giacchi-2EC866?style=flat-square\&logo=hackerrank)](https://www.hackerrank.com/profile/stefanogiacchi)

> *“I build architectures that scale, govern data, and enable intelligent behavior — transforming software into a strategic asset.”*
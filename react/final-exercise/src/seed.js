const faker = require('faker');
const fs = require('fs');
const original = require('../db.json');

const numberOfProjects = 10;
const prjList = [...original.projects];

for (let i = 0; i < numberOfProjects; i++) {
  let prj = {
    id: faker.datatype.uuid(),
    startDate: faker.date.past(),
    endDate: faker.date.future(),
    number: faker.datatype.number(),
    name: faker.commerce.productName(),
    customer: faker.company.companyName(),
    group: faker.random.arrayElement(['.NET', 'Java']),
    members: faker.name.findName(),
    status: faker.datatype.number({ min: 0, max: 4})
  };
  prjList.push(prj);
}

const data = JSON.stringify({ projects: prjList });
fs.writeFileSync('./db.json', data);
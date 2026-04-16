import { expect } from "chai";
import { capitalizeFirstLetter } from "../../helpers";

describe("helpers.capitalizeFirstLetter", () => {
    it("uppercases the first character of a lowercase word", () => {
        expect(capitalizeFirstLetter("pack")).to.equal("Pack");
    });

    it("leaves an already-capitalised word unchanged", () => {
        expect(capitalizeFirstLetter("Pack")).to.equal("Pack");
    });

    it("only touches the first character, not the rest", () => {
        expect(capitalizeFirstLetter("pACK")).to.equal("PACK");
    });

    it("handles a single-character string", () => {
        expect(capitalizeFirstLetter("p")).to.equal("P");
    });

    it("returns an empty string when given an empty string", () => {
        expect(capitalizeFirstLetter("")).to.equal("");
    });

    it("leaves non-letter first characters unchanged", () => {
        expect(capitalizeFirstLetter("1choco")).to.equal("1choco");
    });
});
